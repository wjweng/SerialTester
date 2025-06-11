from PyQt5 import QtCore, QtWidgets
import sys
import serial.tools.list_ports

from serial_comm import SerialComm
from serial_config import SerialConfig
from serial_ui import Ui_SerialWidget

import binascii

VERSION_CMD = bytes([0x81, 0x09, 0x00, 0x02, 0xFF])
CONFIG_FILE = "serial_config.json"


def list_serial_ports():
    return [p.device for p in serial.tools.list_ports.comports()]


class ConfigDialog(QtWidgets.QDialog):
    def __init__(self, parent, config: SerialConfig):
        super().__init__(parent)
        self.setWindowTitle("Configure")
        self.config = config

        layout = QtWidgets.QFormLayout(self)

        self.editBaud = QtWidgets.QLineEdit(str(config.baud_rate))
        layout.addRow("Baud rate:", self.editBaud)

        self.comboParity = QtWidgets.QComboBox()
        self.comboParity.addItems(['N', 'E', 'O', 'M', 'S'])
        self.comboParity.setCurrentText(config.parity)
        layout.addRow("Parity:", self.comboParity)

        self.comboStop = QtWidgets.QComboBox()
        self.comboStop.addItems(['1', '1.5', '2'])
        self.comboStop.setCurrentText(str(config.stop_bits))
        layout.addRow("Stop bits:", self.comboStop)

        self.comboFlow = QtWidgets.QComboBox()
        self.comboFlow.addItems(['none', 'rtscts', 'dsrdtr', 'xonxoff'])
        self.comboFlow.setCurrentText(config.flow_control)
        layout.addRow("Flow control:", self.comboFlow)

        self.buttons = QtWidgets.QDialogButtonBox(
            QtWidgets.QDialogButtonBox.Save | QtWidgets.QDialogButtonBox.Cancel)
        layout.addRow(self.buttons)
        self.buttons.accepted.connect(self.accept)
        self.buttons.rejected.connect(self.reject)

    def accept(self):
        self.config.baud_rate = int(self.editBaud.text())
        self.config.parity = self.comboParity.currentText()
        self.config.stop_bits = float(self.comboStop.currentText())
        self.config.flow_control = self.comboFlow.currentText()
        self.config.save(CONFIG_FILE)
        super().accept()


class SerialWindow(QtWidgets.QWidget):
    data_received = QtCore.pyqtSignal(bytes)

    def __init__(self):
        super().__init__()
        self.ui = Ui_SerialWidget()
        self.ui.setupUi(self)

        self.config_data = SerialConfig()
        self.config_data.load(CONFIG_FILE)
        self.comm = None
        self.connected = False

        self.data_received.connect(self.handle_rx)

        self.refresh_ports()

        self.ui.btnOnline.clicked.connect(self.toggle_connection)
        self.ui.btnConfigure.clicked.connect(self.open_config)
        self.ui.btnClear.clicked.connect(self.clear_text)
        self.ui.btnTest1.clicked.connect(lambda: self.handle_test(1))
        self.ui.btnTest2.clicked.connect(lambda: self.handle_test(2))
        self.ui.btnTest3.clicked.connect(lambda: self.handle_test(3))
        self.ui.btnTest4.clicked.connect(lambda: self.handle_test(4))
        self.ui.btnTest5.clicked.connect(lambda: self.handle_test(5))
        self.ui.btnTest6.clicked.connect(lambda: self.handle_test(6))
        self.ui.btnTest7.clicked.connect(lambda: self.handle_test(7))
        self.ui.btnTest8.clicked.connect(lambda: self.handle_test(8))
        self.ui.btnSetMove.clicked.connect(self.set_move)
        self.ui.btnClearMove.clicked.connect(self.clear_move)
        self.ui.btnSetDefault.clicked.connect(self.set_default_move)
        self.ui.btnShowSpeed.clicked.connect(self.start_speed_timer)
        self.ui.btnStopSpeed.clicked.connect(self.stop_speed_timer)

        self.speed_timer = QtCore.QTimer(self)
        self.speed_timer.timeout.connect(self.send_speed_query)

        # populate combo boxes with basic items
        self.ui.comboAlarmNumber.addItems(["000:Preset", "255:None"])
        self.ui.comboAlarmType.addItems([
            "000:Success Start", "001:Success Termination", "002:End",
            "003:Release", "004:Inspection", "005:Open Store",
            "006:Shut Store", "007:Call For Clerk", "008:Break Wire",
            "009:Abnormal Over-Output", "010:Abnormal Supply",
            "011:Abnormal Recovery", "012:Abnormal Base",
            "013:Abnormal Prize", "014:Abnormal Open-Door",
            "015:Glass Open", "016:Abnormal Communication/Station",
            "021:Abnormal Radio Wave", "022:Abnormal Magnet",
        ])

    def refresh_ports(self):
        ports = list_serial_ports()
        self.ui.comboPort.clear()
        self.ui.comboPort.addItems(ports)
        idx = self.ui.comboPort.findText(self.config_data.port_name)
        if idx >= 0:
            self.ui.comboPort.setCurrentIndex(idx)

    def toggle_connection(self):
        if not self.connected:
            self.config_data.port_name = self.ui.comboPort.currentText()
            self.comm = SerialComm(config=self.config_data, on_rx_char=self.on_rx)
            self.comm.open()
            self.send_command(VERSION_CMD)
            self.ui.btnOnline.setText("OffLine")
            self.connected = True
        else:
            if self.comm:
                self.comm.close()
                self.comm = None
            self.ui.btnOnline.setText("OnLine")
            self.connected = False

    def send_command(self, data: bytes):
        if self.comm:
            self.comm.send(data)
            self.ui.textTx.append(' '.join(f'{b:02X}' for b in data))

    def parse_hex(self, text: str) -> bytes:
        text = text.strip().replace(' ', '')
        if len(text) % 2 != 0:
            text = text[:-1]
        try:
            return bytes.fromhex(text)
        except ValueError:
            return b''

    def handle_test(self, idx: int):
        edit = getattr(self.ui, f'editTest{idx}')
        cmd = self.parse_hex(edit.text())
        if cmd:
            self.send_command(cmd)

    def set_move(self):
        target = self.ui.spinTarget.value()
        dome = self.ui.spinDome.value()
        func = self.ui.spinFunction.value()
        # placeholder command using values
        cmd = bytes([0x81, 0x09, target & 0xFF, func & 0xFF, 0xFF])
        self.send_command(cmd)

    def clear_move(self):
        self.ui.spinTarget.setValue(0)
        self.ui.spinDome.setValue(0)
        self.ui.spinFunction.setValue(0)

    def set_default_move(self):
        self.ui.spinTarget.setValue(0)
        self.ui.spinDome.setValue(1)
        self.ui.spinFunction.setValue(0)

    def start_speed_timer(self):
        self.speed_timer.start(500)

    def stop_speed_timer(self):
        self.speed_timer.stop()

    def send_speed_query(self):
        cmd = bytes([0x81, 0xD9, 0x06, 0x03, 0xFF])
        self.send_command(cmd)

    def on_rx(self, data: bytes):
        """Callback from SerialComm running in background thread."""
        self.data_received.emit(data)

    def handle_rx(self, data: bytes):
        if 0xFF in data:
            idx = data.index(0xFF)
            packet = data[:idx+1]
            if len(packet) >= 8:
                ver = f"{2000 + packet[4]}/{packet[5]}/{packet[6]}-{packet[7]}"
                self.ui.labelFwValue.setText(ver)
        self.ui.textRx.append(' '.join(f'{b:02X}' for b in data))

    def clear_text(self):
        self.ui.textTx.clear()
        self.ui.textRx.clear()

    def open_config(self):
        dlg = ConfigDialog(self, self.config_data)
        if dlg.exec_() == QtWidgets.QDialog.Accepted:
            self.refresh_ports()


def main():
    app = QtWidgets.QApplication(sys.argv)
    win = SerialWindow()
    win.show()
    sys.exit(app.exec_())


if __name__ == "__main__":
    main()

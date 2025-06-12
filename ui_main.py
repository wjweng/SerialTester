from PyQt5 import QtCore, QtWidgets
import sys
import serial.tools.list_ports

from serial_comm import SerialComm
from serial_config import SerialConfig
from serial_ui import Ui_SerialWidget

import binascii

VERSION_CMD = bytes([0x81, 0x09, 0x00, 0x02, 0xFF])
CONFIG_FILE = "serial_config.json"
DEFAULT_SPEED_LEVEL = 100


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
        self.pending_cmd = None

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
        self.ui.btnShowSpeed.clicked.connect(self.start_speed_timer)
        self.ui.btnStopSpeed.clicked.connect(self.stop_speed_timer)
        self.ui.btnTiltUp.clicked.connect(self.tilt_up_clicked)
        self.ui.btnTiltUp.pressed.connect(self.tilt_up_pressed)
        self.ui.btnTiltUp.released.connect(self.tilt_released)
        self.ui.btnTiltDown.clicked.connect(self.tilt_down_clicked)
        self.ui.btnTiltDown.pressed.connect(self.tilt_down_pressed)
        self.ui.btnTiltDown.released.connect(self.tilt_released)
        self.ui.btnPanLeft.clicked.connect(self.pan_left_clicked)
        self.ui.btnPanLeft.pressed.connect(self.pan_left_pressed)
        self.ui.btnPanLeft.released.connect(self.pan_released)
        self.ui.btnPanRight.clicked.connect(self.pan_right_clicked)
        self.ui.btnPanRight.pressed.connect(self.pan_right_pressed)
        self.ui.btnPanRight.released.connect(self.pan_released)
        self.ui.btnPanStop.clicked.connect(self.send_stop_command)
        self.ui.btnStopAt.clicked.connect(self.stop_at)
        self.ui.btnABS.clicked.connect(self.abs_move)
        self.ui.btnABS2.clicked.connect(self.abs_move2)
        self.ui.btnABSAngle.clicked.connect(self.abs_angle)
        self.ui.btnABSAngle2.clicked.connect(self.abs_angle2)
        self.ui.btnABSStop.clicked.connect(self.abs_stop)
        self.ui.btnABSAngleStop.clicked.connect(self.abs_angle_stop)
        self.ui.btnPanType.clicked.connect(self.get_pan_type)
        self.ui.comboPanMethod.currentIndexChanged.connect(self.set_pan_method)
        self.ui.btnHome.clicked.connect(self.go_home)

        self.speed_timer = QtCore.QTimer(self)
        self.speed_timer.timeout.connect(self.send_speed_query)


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
            self.pending_cmd = 'version'
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


    def start_speed_timer(self):
        self.speed_timer.start(500)

    def stop_speed_timer(self):
        self.speed_timer.stop()

    def send_speed_query(self):
        cmd = bytes([0x81, 0xD9, 0x06, 0x03, 0xFF])
        self.send_command(cmd)

    def get_speed_level(self) -> int:
        return DEFAULT_SPEED_LEVEL

    def send_stop_command(self):
        cmd = bytes([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF])
        self.send_command(cmd)

    def tilt_up_clicked(self):
        if self.ui.checkMoveStop.isChecked():
            return

        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x01, 0xFF])
        cmd[5] = level
        self.send_command(bytes(cmd))

    def tilt_up_pressed(self):
        if not self.ui.checkMoveStop.isChecked():
            return
        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x01, 0xFF])
        cmd[5] = level
        self.send_command(bytes(cmd))

    def tilt_down_clicked(self):
        if self.ui.checkMoveStop.isChecked():
            return

        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x02, 0xFF])
        cmd[5] = level
        self.send_command(bytes(cmd))

    def tilt_down_pressed(self):
        if not self.ui.checkMoveStop.isChecked():
            return
        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x02, 0xFF])
        cmd[5] = level
        self.send_command(bytes(cmd))

    def tilt_released(self):
        if self.ui.checkMoveStop.isChecked():
            self.send_stop_command()

    def pan_left_clicked(self):
        if self.ui.checkMoveStop.isChecked():
            return

        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x01, 0x03, 0xFF])
        cmd[4] = level
        self.send_command(bytes(cmd))

    def pan_left_pressed(self):
        if not self.ui.checkMoveStop.isChecked():
            return
        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x01, 0x03, 0xFF])
        cmd[4] = level
        self.send_command(bytes(cmd))

    def pan_right_clicked(self):
        if self.ui.checkMoveStop.isChecked():
            return

        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x02, 0x03, 0xFF])
        cmd[4] = level
        self.send_command(bytes(cmd))

    def pan_right_pressed(self):
        if not self.ui.checkMoveStop.isChecked():
            return
        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x02, 0x03, 0xFF])
        cmd[4] = level
        self.send_command(bytes(cmd))

    def pan_released(self):
        if self.ui.checkMoveStop.isChecked():
            self.send_stop_command()

    def stop_at(self):
        text = self.ui.editStopAt.text() or "0"
        pos = int(text)
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03,
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF])
        cmd[8] = (pos >> 12) & 0x0F
        cmd[9] = (pos >> 8) & 0x0F
        cmd[10] = (pos >> 4) & 0x0F
        cmd[11] = pos & 0x0F
        self.send_command(bytes(cmd))

    def abs_command(self, code: int, edit: QtWidgets.QLineEdit):
        text = edit.text() or "0"
        pos = int(text)
        level = self.get_speed_level()
        cmd = bytearray([0x81, 0x01, 0x06, code, 0x00, 0x00,
                         0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF])
        cmd[4] = level
        cmd[6] = (pos >> 12) & 0x0F
        cmd[7] = (pos >> 8) & 0x0F
        cmd[8] = (pos >> 4) & 0x0F
        cmd[9] = pos & 0x0F
        self.send_command(bytes(cmd))

    def abs_move(self):
        self.abs_command(0x02, self.ui.editABSPos)

    def abs_move2(self):
        self.abs_command(0x02, self.ui.editABS2Pos)

    def abs_angle(self):
        self.abs_command(0x06, self.ui.editABSAngle)

    def abs_angle2(self):
        self.abs_command(0x06, self.ui.editABSAngle2)

    def abs_stop(self):
        cmd = bytes([0x81, 0x01, 0x06, 0x02, 0x00, 0x00, 0xFF])
        self.send_command(cmd)

    def abs_angle_stop(self):
        cmd = bytes([0x81, 0x01, 0x06, 0x06, 0x00, 0x00, 0xFF])
        self.send_command(cmd)

    def get_pan_type(self):
        self.pending_cmd = 'pan_type'
        cmd = bytes([0x81, 0xD9, 0x06, 0x02, 0xFF])
        self.send_command(cmd)

    def set_pan_method(self, idx: int):
        if idx is None:
            idx = self.ui.comboPanMethod.currentIndex()
        if idx < 0:
            idx = 0
            self.ui.comboPanMethod.setCurrentIndex(0)
        cmd = bytes([0x81, 0xD1, 0x06, 0x02, idx & 0x0F, 0xFF])
        self.send_command(cmd)

    def go_home(self):
        cmd = bytes([0x81, 0x01, 0x06, 0x04, 0xFF])
        self.send_command(cmd)

    def on_rx(self, data: bytes):
        """Callback from SerialComm running in background thread."""
        self.data_received.emit(data)

    def handle_rx(self, data: bytes):
        if 0xFF in data:
            idx = data.index(0xFF)
            packet = data[:idx+1]
            if self.pending_cmd == 'pan_type' and len(packet) >= 3:
                value = packet[2] & 0x03
                if value < self.ui.comboPanMethod.count():
                    self.ui.comboPanMethod.setCurrentIndex(value)
                self.pending_cmd = None
            elif self.pending_cmd == 'version' and len(packet) >= 8:
                p5 = f"0{packet[5]}" if packet[5] < 10 else str(packet[5])
                p6 = f"0{packet[6]}" if packet[6] < 10 else str(packet[6])
                p7 = f"0{packet[7]}" if packet[7] < 10 else str(packet[7])
                ver = f"{2000 + packet[4]}{p5}{p6}-{p7}"
                self.ui.labelFwValue.setText(ver)
                self.pending_cmd = None

        # Split data into packets at 0xFF and format each packet
        packets = []
        start = 0
        for i, byte in enumerate(data):
            if byte == 0xFF:
                packet = data[start:i+1]
                packets.append(' '.join(f'{b:02X}' for b in packet))
                start = i+1
        
        # Add any remaining bytes
        if start < len(data):
            packets.append(' '.join(f'{b:02X}' for b in data[start:]))
        
        # Join packets with newlines and append to textRx
        self.ui.textRx.append('\n'.join(packets))

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

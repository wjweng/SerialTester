from PyQt5 import QtCore, QtWidgets, QtChart
from tab_main_ui import Ui_MainTab

class Ui_SerialWidget(object):
    def setupUi(self, SerialWidget):
        SerialWidget.setObjectName("SerialWidget")
        SerialWidget.resize(600, 400)

        self.verticalLayout = QtWidgets.QVBoxLayout(SerialWidget)
        self.verticalLayout.setContentsMargins(5, 5, 5, 5)

        self.topLayout = QtWidgets.QHBoxLayout()

        self.btnOnline = QtWidgets.QPushButton(SerialWidget)
        self.btnOnline.setObjectName("btnOnline")
        self.topLayout.addWidget(self.btnOnline)

        self.labelComPort = QtWidgets.QLabel(SerialWidget)
        self.labelComPort.setObjectName("labelComPort")
        self.topLayout.addWidget(self.labelComPort)

        self.comboPort = QtWidgets.QComboBox(SerialWidget)
        self.comboPort.setObjectName("comboPort")
        self.topLayout.addWidget(self.comboPort)

        self.btnConfigure = QtWidgets.QPushButton(SerialWidget)
        self.btnConfigure.setObjectName("btnConfigure")
        self.topLayout.addWidget(self.btnConfigure)

        self.labelFw = QtWidgets.QLabel(SerialWidget)
        self.labelFw.setObjectName("labelFw")
        self.topLayout.addWidget(self.labelFw)

        self.labelFwValue = QtWidgets.QLabel(SerialWidget)
        self.labelFwValue.setObjectName("labelFwValue")
        self.labelFwValue.setMinimumWidth(100)
        self.labelFwValue.setFrameShape(QtWidgets.QFrame.Panel)
        self.labelFwValue.setFrameShadow(QtWidgets.QFrame.Sunken)
        self.topLayout.addWidget(self.labelFwValue)

        self.verticalLayout.addLayout(self.topLayout)

        self.tabWidget = QtWidgets.QTabWidget(SerialWidget)
        self.tabWidget.setObjectName("tabWidget")

        self.tabMain = QtWidgets.QWidget()
        self.tabMain.setObjectName("tabMain")
        self.tabMainUi = Ui_MainTab()
        self.tabMainUi.setupUi(self.tabMain)
        # expose widgets from main tab for external access
        for name in [
            'btnTiltUp', 'btnTiltDown', 'btnPanLeft', 'btnPanRight',
            'btnPanStop', 'btnStopAt', 'checkMoveStop', 'editStopAt',
            'btnABS', 'btnABS2', 'btnABSAngle', 'btnABSAngle2',
            'btnABSStop', 'btnPanType', 'comboPanMethod', 'btnABSAngleStop',
            'editABSPos', 'editABS2Pos', 'editABSAngle', 'editABSAngle2',
            'btnRelUp', 'btnRelDown', 'btnRelLeft', 'btnRelRight', 'btnRelStop',
            'editRelStep', 'btnHome',
            'chartSpeed', 'btnShowSpeed', 'btnStopSpeed', 'btnClearChart']:
            setattr(self, name, getattr(self.tabMainUi, name))
        self.tabWidget.addTab(self.tabMain, "")

        self.tabTest = QtWidgets.QWidget()
        self.tabTest.setObjectName("tabTest")
        self.verticalLayoutTest = QtWidgets.QVBoxLayout(self.tabTest)
        self.verticalLayoutTest.setObjectName("verticalLayoutTest")
        self.groupVisca = QtWidgets.QGroupBox(self.tabTest)
        self.groupVisca.setObjectName("groupVisca")
        self.gridVisca = QtWidgets.QGridLayout(self.groupVisca)
        self.gridVisca.setObjectName("gridVisca")

        self.btnTest1 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest1.setObjectName("btnTest1")
        self.gridVisca.addWidget(self.btnTest1, 0, 0, 1, 1)
        self.editTest1 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest1.setObjectName("editTest1")
        self.gridVisca.addWidget(self.editTest1, 0, 1, 1, 1)

        self.btnTest2 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest2.setObjectName("btnTest2")
        self.gridVisca.addWidget(self.btnTest2, 1, 0, 1, 1)
        self.editTest2 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest2.setObjectName("editTest2")
        self.gridVisca.addWidget(self.editTest2, 1, 1, 1, 1)

        self.btnTest3 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest3.setObjectName("btnTest3")
        self.gridVisca.addWidget(self.btnTest3, 2, 0, 1, 1)
        self.editTest3 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest3.setObjectName("editTest3")
        self.gridVisca.addWidget(self.editTest3, 2, 1, 1, 1)

        self.btnTest4 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest4.setObjectName("btnTest4")
        self.gridVisca.addWidget(self.btnTest4, 3, 0, 1, 1)
        self.editTest4 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest4.setObjectName("editTest4")
        self.gridVisca.addWidget(self.editTest4, 3, 1, 1, 1)

        self.btnTest5 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest5.setObjectName("btnTest5")
        self.gridVisca.addWidget(self.btnTest5, 4, 0, 1, 1)
        self.editTest5 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest5.setObjectName("editTest5")
        self.gridVisca.addWidget(self.editTest5, 4, 1, 1, 1)

        self.btnTest6 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest6.setObjectName("btnTest6")
        self.gridVisca.addWidget(self.btnTest6, 5, 0, 1, 1)
        self.editTest6 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest6.setObjectName("editTest6")
        self.gridVisca.addWidget(self.editTest6, 5, 1, 1, 1)

        self.btnTest7 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest7.setObjectName("btnTest7")
        self.gridVisca.addWidget(self.btnTest7, 6, 0, 1, 1)
        self.editTest7 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest7.setObjectName("editTest7")
        self.gridVisca.addWidget(self.editTest7, 6, 1, 1, 1)

        self.btnTest8 = QtWidgets.QPushButton(self.groupVisca)
        self.btnTest8.setObjectName("btnTest8")
        self.gridVisca.addWidget(self.btnTest8, 7, 0, 1, 1)
        self.editTest8 = QtWidgets.QLineEdit(self.groupVisca)
        self.editTest8.setObjectName("editTest8")
        self.gridVisca.addWidget(self.editTest8, 7, 1, 1, 1)

        self.verticalLayoutTest.addWidget(self.groupVisca)
        self.tabTest.setLayout(self.verticalLayoutTest)
        self.tabWidget.addTab(self.tabTest, "")

        self.verticalLayout.addWidget(self.tabWidget)

        self.formLayout = QtWidgets.QFormLayout()
        self.formLayout.setObjectName("formLayout")

        self.labelTx = QtWidgets.QLabel(SerialWidget)
        self.labelTx.setObjectName("labelTx")
        self.formLayout.setWidget(0, QtWidgets.QFormLayout.LabelRole, self.labelTx)

        self.textTx = QtWidgets.QTextEdit(SerialWidget)
        self.textTx.setObjectName("textTx")
        self.formLayout.setWidget(0, QtWidgets.QFormLayout.FieldRole, self.textTx)

        self.labelRx = QtWidgets.QLabel(SerialWidget)
        self.labelRx.setObjectName("labelRx")
        self.formLayout.setWidget(1, QtWidgets.QFormLayout.LabelRole, self.labelRx)

        self.textRx = QtWidgets.QTextEdit(SerialWidget)
        self.textRx.setObjectName("textRx")
        self.formLayout.setWidget(1, QtWidgets.QFormLayout.FieldRole, self.textRx)

        self.verticalLayout.addLayout(self.formLayout)

        self.btnClear = QtWidgets.QPushButton(SerialWidget)
        self.btnClear.setObjectName("btnClear")
        self.verticalLayout.addWidget(self.btnClear)

        self.retranslateUi(SerialWidget)
        QtCore.QMetaObject.connectSlotsByName(SerialWidget)

    def retranslateUi(self, SerialWidget):
        _translate = QtCore.QCoreApplication.translate
        SerialWidget.setWindowTitle(_translate("SerialWidget", "Serial Tester"))
        self.btnOnline.setText(_translate("SerialWidget", "OnLine"))
        self.labelComPort.setText(_translate("SerialWidget", "ComPort:"))
        self.btnConfigure.setText(_translate("SerialWidget", "Configure"))
        self.labelFw.setText(_translate("SerialWidget", "FW Version:"))
        self.labelFwValue.setText("")
        self.tabWidget.setTabText(self.tabWidget.indexOf(self.tabMain), _translate("SerialWidget", "Main"))
        self.tabWidget.setTabText(self.tabWidget.indexOf(self.tabTest), _translate("SerialWidget", "Test"))
        self.btnShowSpeed.setText(_translate("SerialWidget", "Show Speed"))
        self.btnStopSpeed.setText(_translate("SerialWidget", "Stop"))
        self.groupVisca.setTitle(_translate("SerialWidget", "Test Visca Commands"))
        self.btnTest1.setText(_translate("SerialWidget", "Test 1"))
        self.btnTest2.setText(_translate("SerialWidget", "Test 2"))
        self.btnTest3.setText(_translate("SerialWidget", "Test 3"))
        self.btnTest4.setText(_translate("SerialWidget", "Test 4"))
        self.btnTest5.setText(_translate("SerialWidget", "Test 5"))
        self.btnTest6.setText(_translate("SerialWidget", "Test 6"))
        self.btnTest7.setText(_translate("SerialWidget", "Test 7"))
        self.btnTest8.setText(_translate("SerialWidget", "Test 8"))
        self.labelTx.setText(_translate("SerialWidget", "Tx:"))
        self.labelRx.setText(_translate("SerialWidget", "Rx:"))
        self.btnClear.setText(_translate("SerialWidget", "Clear"))

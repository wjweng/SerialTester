# New file: pan_tilt_controller.py
"""High level controller for pan/tilt units."""

from __future__ import annotations

from typing import Callable, Optional

from serial_comm import SerialComm
from serial_config import SerialConfig
from protocol import ProtocolParser, ParseResult


class PanTiltController:
    """Wrap :class:`SerialComm` with higher level command helpers."""

    def __init__(self, config: Optional[SerialConfig] = None) -> None:
        self.comm = SerialComm(config=config or SerialConfig(),
                               on_rx_char=self._on_rx)
        self.parser = ProtocolParser()
        self.buffer = bytearray()
        self.pending_cmd: Optional[str] = None
        self.on_result: Callable[[ParseResult], None] = lambda res: None
        self.on_raw: Callable[[bytes], None] = lambda data: None

    # --- serial management -------------------------------------------------
    def open(self) -> None:
        self.comm.open()

    def close(self) -> None:
        self.comm.close()

    # --- internal helpers --------------------------------------------------
    def _on_rx(self, data: bytes) -> None:
        """Handle incoming data from :class:`SerialComm`."""
        # forward raw data first
        self.on_raw(data)
        self.buffer.extend(data)
        while 0xFF in self.buffer:
            idx = self.buffer.index(0xFF)
            packet = bytes(self.buffer[:idx + 1])
            del self.buffer[:idx + 1]
            result = self.parser.parse(packet, self.pending_cmd)
            if result:
                self.pending_cmd = None
                self.on_result(result)

    def send(self, data: bytes, pending: Optional[str] = None) -> None:
        self.pending_cmd = pending
        self.comm.send(data)

    # --- high level commands ----------------------------------------------
    def stop(self) -> None:
        cmd = bytes([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03, 0xFF])
        self.send(cmd)

    def abs_move(self, position: int, speed: int) -> None:
        """Move to absolute *position* at *speed*."""
        cmd = bytearray([0x81, 0x01, 0x06, 0x02, 0, 0,
                         0, 0, 0, 0, 0, 0, 0, 0, 0xFF])
        cmd[4] = speed
        cmd[6] = (position >> 12) & 0x0F
        cmd[7] = (position >> 8) & 0x0F
        cmd[8] = (position >> 4) & 0x0F
        cmd[9] = position & 0x0F
        self.send(bytes(cmd))

    def rel_move(self, direction: str, step: int, speed: int) -> None:
        """Move relatively in *direction* by *step*."""
        cmd = bytearray([0x81, 0x01, 0x06, 0x03, 0, 0,
                         0, 0, 0, 0, 0, 0, 0, 0, 0, 0xFF])
        if direction in ("left", "right"):
            cmd[4] = speed
            cmd[6] = 0x00 if direction == "left" else 0x01
            cmd[7] = (step >> 12) & 0x0F
            cmd[8] = (step >> 8) & 0x0F
            cmd[9] = (step >> 4) & 0x0F
            cmd[10] = step & 0x0F
        else:
            cmd[5] = speed
            cmd[11] = 0x00 if direction == "up" else 0x01
            cmd[12] = (step >> 12) & 0x0F
            cmd[13] = (step >> 8) & 0x0F
            cmd[14] = (step >> 4) & 0x0F
            cmd[15] = step & 0x0F
        self.send(bytes(cmd))

    def get_speed(self) -> None:
        cmd = bytes([0x81, 0xD9, 0x06, 0x03, 0xFF])
        self.send(cmd, pending="current_speed")

    # ----- additional command helpers -------------------------------------
    def get_version(self) -> None:
        """Query firmware version."""
        cmd = bytes([0x81, 0x09, 0x00, 0x02, 0xFF])
        self.send(cmd, pending="version")

    def get_mcu_type(self) -> None:
        cmd = bytes([0x81, 0x09, 0x00, 0x03, 0xFF])
        self.send(cmd, pending="mcu_type")

    def get_pan_type(self) -> None:
        cmd = bytes([0x81, 0xD9, 0x06, 0x02, 0xFF])
        self.send(cmd, pending="pan_type")

    def set_pan_method(self, idx: int) -> None:
        cmd = bytes([0x81, 0xD1, 0x06, 0x02, idx & 0x0F, 0xFF])
        self.send(cmd)

    def tilt_up(self, speed: int) -> None:
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x01, 0xFF])
        cmd[5] = speed
        self.send(bytes(cmd))

    def tilt_down(self, speed: int) -> None:
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x02, 0xFF])
        cmd[5] = speed
        self.send(bytes(cmd))

    def pan_left(self, speed: int) -> None:
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x01, 0x03, 0xFF])
        cmd[4] = speed
        self.send(bytes(cmd))

    def pan_right(self, speed: int) -> None:
        cmd = bytearray([0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x02, 0x03, 0xFF])
        cmd[4] = speed
        self.send(bytes(cmd))

    def stop_at(self, position: int) -> None:
        cmd = bytearray([
            0x81, 0x01, 0x06, 0x01, 0x00, 0x00, 0x03, 0x03,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF
        ])
        cmd[8] = (position >> 12) & 0x0F
        cmd[9] = (position >> 8) & 0x0F
        cmd[10] = (position >> 4) & 0x0F
        cmd[11] = position & 0x0F
        self.send(bytes(cmd))

    def stall_cali_on(self) -> None:
        self.send(bytes([0x81, 0xD1, 0x06, 0x05, 0x02, 0xFF]))

    def stall_cali_off(self) -> None:
        self.send(bytes([0x81, 0xD1, 0x06, 0x05, 0x03, 0xFF]))

    def zero_cali_plus(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x05, 0x01, 0x00, 0xFF]))

    def zero_cali_minus(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x05, 0x01, 0x02, 0xFF]))

    def clear_zero_cali(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x05, 0x00, 0xFF]))

    def zero_cali_status(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x05, 0x55, 0xFF]), pending="zp_status")

    def lock_home(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x04, 0x01, 0xFF]))

    def unlock_home(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x04, 0x00, 0xFF]))

    def lock_status(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x05, 0x56, 0xFF]), pending="lock_status")

    def go_home(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x04, 0xFF]))

    def set_speed_level(self, level: int) -> None:
        cmd = bytes([0x81, 0xD9, 0x06, 0x04, level & 0xFF, 0xFF])
        self.send(cmd, pending="speed_pps")

    def get_speed_by_zoom(self) -> None:
        self.send(bytes([0x81, 0x09, 0x06, 0xA2, 0xFF]), pending="speed_zoom")

    def speed_by_zoom_on(self, ratio: int) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0xA2, 0x02, ratio & 0xFF, 0xFF]))

    def speed_by_zoom_off(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0xA2, 0x03, 0xFF]))

    def set_target_speed(self, value: int) -> None:
        cmd = bytearray([0x81, 0xD1, 0x06, 0x03, 0, 0, 0, 0, 0xFF])
        cmd[4] = (value >> 12) & 0x0F
        cmd[5] = (value >> 8) & 0x0F
        cmd[6] = (value >> 4) & 0x0F
        cmd[7] = value & 0x0F
        self.send(bytes(cmd))

    def get_acceleration(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x06, 0x01, 0xFF]), pending="acc_value")

    def set_acceleration(self, value: int) -> None:
        cmd = bytearray([0x81, 0xD1, 0x06, 0x01, 0, 0, 0, 0, 0xFF])
        cmd[4] = (value >> 12) & 0x0F
        cmd[5] = (value >> 8) & 0x0F
        cmd[6] = (value >> 4) & 0x0F
        cmd[7] = value & 0x0F
        self.send(bytes(cmd))

    def get_acc_level(self) -> None:
        self.send(bytes([0x81, 0x09, 0x06, 0x31, 0xFF]), pending="acc_level")

    def set_acc_level(self, idx: int) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x31, (idx + 1) & 0x0F, 0xFF]))

    def get_position(self) -> None:
        self.send(bytes([0x81, 0x09, 0x06, 0x12, 0xFF]), pending="position")

    def get_angle(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x05, 0x51, 0xFF]), pending="angle")

    def get_ab_count(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x05, 0x52, 0xFF]), pending="ab_count")

    def get_z_count(self) -> None:
        self.send(bytes([0x81, 0xD9, 0x05, 0x53, 0xFF]), pending="z_count")

    def max_angle_on(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x66, 0x02, 0xFF]))

    def max_angle_off(self) -> None:
        self.send(bytes([0x81, 0x01, 0x06, 0x66, 0x03, 0xFF]))

    def motor_type_0p9d(self) -> None:
        self.send(bytes([0x81, 0x01, 0x00, 0x03, 0x00, 0xFF]))

    def motor_type_1p8d(self) -> None:
        self.send(bytes([0x81, 0x01, 0x00, 0x03, 0x01, 0xFF]))

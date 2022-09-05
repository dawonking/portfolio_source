import os
from tkinter import *
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler
from watchdog.events import PatternMatchingEventHandler
import time
from PIL import Image
import cv2
from cv2 import aruco
import shutil

import numpy as np

import threading
import struct
import socket

scan_name = 0
#프로젝터 pc ip
serverAddressPort = ("192.168.0.116",6231 )
UDPClientSocket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

ScannerFolder = 'C:/Users/Public/House'

class Watcher:
    DIRECTORY_WATCH = ScannerFolder
    #실행시 기존 폴더에 있던 파일 전체 삭제
    if os.path.exists(ScannerFolder):
        for file in os.scandir(ScannerFolder):
            os.remove(file.path)
            print("remove")
    else:
        print("NO file")

    def __init__(self):
        self.observer = Observer()

    def run(self):
        event_handler = Handler()
        
        self.observer.schedule(event_handler, self.DIRECTORY_WATCH, recursive=True)
        self.observer.start()
        print("check")
        try:
            while True:
                time.sleep(5)

        except:
            self.observer.stop()
            print("Error")
            self.observer.join()


class Handler(FileSystemEventHandler):
    global result
    #파일 생성확인
    def on_created(self, event):
        try:
            time.sleep(1.0)
            print("Created")
            file_list = os.listdir(ScannerFolder)
            file_list.sort(reverse=True)
            Recent_File_Path = ScannerFolder + '/' + file_list[0]
            print(Recent_File_Path)

            waitting = True
            timecount = 0

            #파일이 생성될때 까지 기다림
            while (waitting):
                file_size = os.path.getsize(Recent_File_Path)
                if (file_size < 20000):
                    print('waitting... = ', timecount, ' / fime_size = ', file_size)
                    time.sleep(0.1)
                    timecount += 0.1

                    if (timecount > 10):
                        print('time_over = ', timecount)
                        waitting = False
                else:
                    print('print_end / ', timecount)
                    waitting = False

            frame = cv2.imread(Recent_File_Path)
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
            aruco_dict = aruco.Dictionary_get(aruco.DICT_6X6_250)
            parameters = aruco.DetectorParameters_create()
            corners, ids, rejectedImgPoints = aruco.detectMarkers(gray, aruco_dict, parameters=parameters)
            Result = ids;
            print('check_id = ', Result)
            time.sleep(0.1)
            
            Copy_Path2 = '//192.168.0.116/Public\House_copy/1.png'
            Ias = cv2.imread(Recent_File_Path)            
            cv2.imwrite(Copy_Path2, Ias, [cv2.IMWRITE_PNG_STRATEGY_HUFFMAN_ONLY, 10])

            time.sleep(0.1)
            Id_Check(ids)
            time.sleep(0.1)

        except:
            print('break')

    #파일삭제확인
    def on_deleted(self, event):
        print("Delet")

#scan_image_id_check
def Id_Check(x):
    try:
        if x == 34:
            print('ligth_house_1')
            temp = 0
            data = bytearray([scan_name]+[temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 33:
            print('light_house_2')
            temp = 1
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 32:
            print('light_house_3')
            temp = 2
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 31:
            print('light_house_4')
            temp = 3
            data = bytearray([scan_name] + [temp])

            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 30:
            print('turtle')
            temp = 4
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 29:
            print('octo')
            temp = 5
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 28:
            print('boat')
            temp = 6
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 27:
            print('yoacht')
            temp = 7
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 26:
            print('squid')
            temp = 8
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 25:
            print('princess')
            temp = 9
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        if x == 24:
            print('submarine')
            temp = 10
            data = bytearray([scan_name] + [temp])
            UDPClientSocket.sendto(data, serverAddressPort)
        else:
            print('Not_Found')
    except:
        print('Null')
    time.sleep(0.2)


if __name__ == '__main__':
    w = Watcher()
    w.run()
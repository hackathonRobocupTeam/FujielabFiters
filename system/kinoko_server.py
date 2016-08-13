#!/usr/bin/env python
# encoding: utf-8
###########################
# Author: Yuya Aoki
#
###########################

import socket
import sys
from threading import Thread
import copy
import time
import client

HOST = "localhost"   # Symbolic name meaning all available interfaces
PORT = 2828          # Arbitrary non-privileged port
stop_flag = True
s = None
list = {}
modules = {}
time_stamps = {}


def return_UNIX_time():
    return time.time()

def sock_server():
    global stop_flag
    while stop_flag:
        conn, addr = s.accept()
        while 1:
            # 受信バイト数。特に意味はない
            data = conn.recv(2048)
            param = dict([tmp.split('=') for tmp in data.split('&')])
            if not data: break
            # ここにdataからroleを引き出してそれぞれに応じた関数をとれるように書く
            if 'role=update' in data:
                __update(param, conn)
            if 'role=pull' in data:
                __pull(param, conn)
            if 'role=push' in data:
                __push(param, conn)
            if "stop" in data:
                sys.exit(0)
    conn.close()

# リクエストに応じた情報を追加
def __push(paramater, conn):
    global list, time_stamps
    if paramater['status_name'] in list:
         list[paramater['status_name']] = paramater['status']
         time_stamps[paramater['status_name']] = return_UNIX_time()
    else:
        list.update({paramater['status_name']: paramater['status']})
        time_stamps.update({paramater['status_name']: return_UNIX_time() })
    res = 'SUCCESS'
    conn.send(res)

# リクエストされた情報の現在の値を返す
def __pull(paramater, conn):
    global list
    if paramater['status_name'] in list:
        res = list[paramater['status_name']]
    else:
        res = 'NON_STATUS'
    conn.send(res)

# 情報に更新があったかを見る
def __update(paramater, conn):
    global list, modules, time_stamps
    module = paramater['module_name']
    status_name = paramater['status_name']
    # status がない場合登録
    if not status_name in time_stamps:
        time_stamps.update({status_name: return_UNIX_time()})
    if module in modules:
        if not status_name in modules[module]:
            modules[module].update({status_name: return_UNIX_time()})
        if paramater['time'] != "-1":
            time.sleep(float(paramater['time']))
        else : # 更新があるまでストップしたい
            while True:
               if time_stamps[status_name] != modules[module][status_name]: return True
        # time_stampの比較
        if modules[module][status_name] < time_stamps[status_name]:
            modules[module][status_name] = copy.deepcopy(time_stamps[status_name])
            res = True
        else:
            res = False
   # moduleの登録
    else:
       modules.update({module: {status_name: copy.deepcopy(time_stamps[status_name])}})
       res = False
    conn.send(res)

# initialize
for res in socket.getaddrinfo(HOST, PORT, socket.AF_UNSPEC,
                              socket.SOCK_STREAM, 0, socket.AI_PASSIVE):
    af, socktype, proto, canonname, sa = res
    try:
       s = socket.socket(af, socktype, proto)
    except socket.error, msg:
       s = None
       continue
    try:
       s.bind(sa)
       s.listen(1)
    except socket.error, msg:
       s.close()
       s = None
       continue
    break
if s is None:
    print 'could not open socket'
    sys.exit(1)

t = Thread(target=sock_server)
t.damon = True
t.start()
while True:
     cmd = raw_input()
     if "list" == cmd:
        print (list)
     if 'modules' == cmd:
        print(modules)
     if 'times' == cmd:
        print time_stamps
     if 'stop' == cmd:
        stop_flag = False
        print "stop"
        client.stop()
        sys.exit(0)


#!/usr/bin/env python
# encoding: utf-8
###########################
# Author: Yuya Aoki
#
###########################

from threading import Thread
# import datetime
import time
import copy
import BaseHTTPServer
import urlparse

list = {}
modules = {}
time_stamps = {}

# request handler
class HttpHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    """docstring for HttpHandler"""

    # response を書く場所
    def do_GET(self):
        parsed_path = urlparse.urlparse(self.path)
        try:        # 4は変数を見ている
            paramater = dict([p.split('=') for p in parsed_path[4].split('%26')])
        except:
            paramater = {}
        if len(paramater) > 0:
            if 'push' in paramater['role']:
                item = self.__push(paramater)
            if 'pull' in paramater['role']:
                item = self.__pull(paramater) 
            if 'update' in paramater['role']:
                item = self.__update(paramater)
        else:
            item = "Error"
        print(item)
        self.send_response(200)
        self.end_headers()
        self.wfile.write(item)

    # リクエストに応じた情報を追加
    def __push(self, paramater):
        global list, time_stamps
        if paramater['status_name'] in list:
             list[paramater['status_name']] = paramater['status']
             time_stamps[paramater['status_name']] = return_UNIX_time()

        else:
            list.update({paramater['status_name']: paramater['status']})
            time_stamps.update({paramater['status_name']: return_UNIX_time() })
        return 'SUCCESS'

    # リクエストされた情報の現在の値を返す
    def __pull(self, paramater):
        global list
        if paramater['status_name'] in list:
            return list[paramater['status_name']]
        else:
            return 'NON_STATUS'

    # 情報に更新があったかを見る
    def __update(self, paramater):
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
                # while True:
                #    if time_stamps[status_name] != modules[module][status_name]: return True
                pass
            # time_stampの比較
            if modules[module][status_name] < time_stamps[status_name]:
                modules[module][status_name] = copy.deepcopy(time_stamps[status_name])
                return True
            else:
                return False

        # moduleの登録
        else:
           modules.update({module: {status_name: copy.deepcopy(time_stamps[status_name])}})
           return False

def return_UNIX_time():
    return time.time()


def makeHttpServer(IP=None):
    if IP is None:
        server_address = ('', 2828)
    else:
        server_address = ()
    httpd = BaseHTTPServer.HTTPServer(server_address, HttpHandler)
    httpd.serve_forever()

if __name__ == '__main__':
    # code
    t = Thread(target=makeHttpServer)
    t.damon = True
    t.start()
    while True:
        cmd = raw_input()
        if "list" == cmd:
            print (list)
        if 'modules' == cmd:
            print(modules)
        if 'stamp' == cmd:
            print time_stamps


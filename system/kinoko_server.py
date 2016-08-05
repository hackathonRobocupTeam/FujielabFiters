#!/usr/bin/env python
# encoding: utf-8
###########################
# Author: Yuya Aoki
#
###########################

from threading import Thread
# import re
# import datetime
# import os
import time
import copy
import BaseHTTPServer
import urlparse

list = {}
modules = {}


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
            if 'commit' in paramater['role']:
                item = self.__commit(paramater)
            if 'pull' in paramater['role']:
                item = self.__pull(paramater) 
            if 'update' in paramater['role']:
                item = self.__update(paramater)
        else:
            item = "Error"
        print item
        self.send_response(200)
        self.end_headers()
        self.wfile.write(item)

    # リクエストに応じた情報を追加
    def __commit(self, paramater):
        global list
        if paramater['status_name'] in list:
             list[paramater['status_name']] = paramater['status']
        else:
            list.update({paramater['status_name']: paramater['status']})
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
        global list, modules
        module = paramater['module_name']
        status_name = paramater['status_name']
        if module in modules:
            if paramater['time'] == "-1":
                # 即座にreturn
                pass
            else:
                time.sleep(float(paramater['time']))
            # 古いデータと今のデータの比較
            if modules[module][status_name] is list[status_name]:
                return False
            else:
                return True
        # moduleの登録
        else:
            # status がない場合登録
            if not paramater['status_name'] in list:
                list.update({paramater['status_name']: "None"})
            modules.update({paramater['module_name']: [paramater['status_name'], copy.deepcopy(list[paramater['status_name']])]})
            return False

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
            print list


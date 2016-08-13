# Echo client program
import socket
import sys

HOST = 'localhost'    # The remote host
PORT = 2828              # The same port as used by the server
s = None
def stop():
    for res in socket.getaddrinfo(HOST, PORT, socket.AF_UNSPEC, socket.SOCK_STREAM):
        af, socktype, proto, canonname, sa = res
        try:
           s = socket.socket(af, socktype, proto)
        except socket.error, msg:
           s = None
           continue
        try:
           s.connect(sa)
        except socket.error, msg:
           s.close()
           s = None
           continue
        break
    if s is None:
        print 'could not open socket'
        sys.exit(1)
    s.send('stop')
    data = s.recv(1024)
    s.close()

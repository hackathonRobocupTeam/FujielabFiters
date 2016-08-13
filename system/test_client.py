# Echo client program
import socket
import sys

HOST = 'localhost'    # The remote host
PORT = 2828              # The same port as used by the server

def print_res(s):
    data = s.recv(2048)
    print data


if __name__ == "__main__":
    s = None
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


    s.send('role=push&status_name=state&status=value')
    print_res(s)
    s.send('role=pull&status_name=state')
    print_res(s)
    s.send('role=update&module_name=test_module&status_name=state&time=1')
    print_res(s)
    s.close()



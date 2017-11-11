__author__ = 'Lidor'
import socket
import select
import Queue

PORT = 8820
HOST = '127.0.0.1'

server = socket.socket()
server.setblocking(0)
server.bind((HOST,PORT))
client = []

while True:
    print "new game loop"
    clients_connected = 0
    server.listen(2)

    #Sockets from which we expect to read or write to
    inputs = [server]

    #waiting for both the connextion to be established
    while clients_connected < 2:
        readable, writeable, exceptional = select.select(inputs,inputs,inputs)
        #handle inputs
        for s in readable:
            if s is server:
            #A readable server socket is ready to accept a connection
                conn, addr = s.accept()
                print "one connection has been established"
                conn.setblocking(0)
                inputs.append(conn)
                clients_connected = clients_connected + 1
            else:
                data = s.recv(4)
                if not data:
                    #Interpert empty result as closed connection
                    print "closing"
                    #stop listening for input and output
                    inputs.remove(s)
                    s.close()
                    clients_connected = clients_connected - 1

    print "both connections has been established"
    inputs.remove(server)

    initiated = 0
    #making sure both of the clients got initializtion message "0000"
    while initiated < 2:
        readable, writeable, exceptional = select.select(inputs,inputs,inputs)
        for s in writeable:
            s.send("0000")
            initiated = initiated + 1
    print "game started"
    #Main loop - forwarding messages between the clients
    while inputs:
        readable, writeable, exceptional = select.select(inputs,inputs,inputs)
        #handle inputs
        for s in readable:
            if s is server:
                print "ERROR! can't initiate another connection after the game has been started!"
            #Recieve data from the client
            data = s.recv(4)
            if data:
                #A readable client socket has data
                print "recieved",data,"from",s.getpeername()
                if data != "0000":
                    #making sure the data isn't initialization message, in case it is the sever will ignore it
                    #broadcast the recived message:
                    for o in inputs:
                        if o is not s:
                            o.send(data)

            else:
                #Interpert empty result as closed connection
                print "closing"
                #stop listening for input and output
                inputs.remove(s)
                s.close()

        for s in exceptional:
            print "handling exceptions for",s.getpeername()
            inputs.remove(s)
            s.close()


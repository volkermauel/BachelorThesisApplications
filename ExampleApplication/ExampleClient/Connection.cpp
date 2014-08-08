#include "Connection.h"



Connection::Connection(char* server,char* port)
{
	WSADATA wsaData;

	this->ConnectSocket = INVALID_SOCKET;
	struct addrinfo *result = NULL,
		*ptr = NULL,
		hints;

	int iResult;




	// Initialize Winsock
	iResult = WSAStartup(MAKEWORD(2, 2), &wsaData);
	if (iResult != 0) {
		printf("WSAStartup failed with error: %d\n", iResult);
		throw 1;
	}

	ZeroMemory(&hints, sizeof(hints));
	hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_protocol = IPPROTO_TCP;

	// Resolve the server address and port
	iResult = getaddrinfo(server, port, &hints, &result);
	if (iResult != 0) {
		printf("getaddrinfo failed with error: %d\n", iResult);
		WSACleanup();
		throw 1;
	}

	// Attempt to connect to an address until one succeeds
	for (ptr = result; ptr != NULL; ptr = ptr->ai_next) {

		// Create a SOCKET for connecting to server
		this->ConnectSocket = socket(ptr->ai_family, ptr->ai_socktype,
			ptr->ai_protocol);
		if (this->ConnectSocket == INVALID_SOCKET) {
			printf("socket failed with error: %ld\n", WSAGetLastError());
			WSACleanup();
			throw 1;
		}

		// Connect to server.
		iResult = connect(this->ConnectSocket, ptr->ai_addr, (int) ptr->ai_addrlen);
		if (iResult == SOCKET_ERROR) {
			closesocket(this->ConnectSocket);
			this->ConnectSocket = INVALID_SOCKET;
			continue;
		}
		break;
	}

	freeaddrinfo(result);

	if (this->ConnectSocket == INVALID_SOCKET) {
		printf("Unable to connect to server!\n");
		WSACleanup();
		throw 1;
	}


}

void Connection::Send(char* buffer, int length){
	// Send a buffer
	int iResult = send(this->ConnectSocket, buffer, length, 0);
	if (iResult == SOCKET_ERROR) {
		printf("send failed with error: %d\n", WSAGetLastError());
		closesocket(this->ConnectSocket);
		WSACleanup();
		throw 1;
	}

	//printf("Bytes Sent: %ld\n", iResult);
}

void Connection::Receive(char* recvbuf,int recvbuflen){
	int iResult;
	int offset = 0;
	do {

		iResult = recv(this->ConnectSocket, recvbuf+offset, recvbuflen, 0);
		if (iResult > 0){
			offset += iResult;
			if (offset == recvbuflen){
				return;
			}
		}
		else if (iResult == 0){
			printf("Connection closed\n");
		}
		else{
			printf("recv failed with error: %d\n", WSAGetLastError());
		}

	} while (iResult > 0);
}

Connection::~Connection()
{
	int iResult = shutdown(this->ConnectSocket, SD_SEND);
	if (iResult == SOCKET_ERROR) {
		printf("shutdown failed with error: %d\n", WSAGetLastError());
		closesocket(this->ConnectSocket);
		WSACleanup();
		throw 1;
	}
	closesocket(this->ConnectSocket);
	WSACleanup();
}

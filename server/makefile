
all: login room table

login: proto/login.proto
	protoc --descriptor_set_out proto/login.pb proto/login.proto

room: proto/room.proto
	protoc --descriptor_set_out proto/room.pb proto/room.proto

table: proto/table.proto
	protoc --descriptor_set_out proto/table.pb proto/table.proto

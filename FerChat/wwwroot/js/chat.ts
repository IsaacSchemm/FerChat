///<reference path="../lib/knockout/build/types/knockout.d.ts" />

declare var signalR: any;

class ChatMessage {
    constructor(readonly user: string, readonly message: string) { }
}

class ChatRoom {
    readonly enabled = ko.observable(false);
    readonly chatMessages = ko.observableArray<ChatMessage>();
    readonly textEntry = ko.observable("");

    constructor(readonly chatRoomId: string) { }

    async load() {

    }

    async send() {
        try {
            await connection.invoke("SendMessage", this.chatRoomId, this.textEntry());
            this.textEntry("");
        } catch (e) {
            console.error(e);
        }
    }
}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

var chatRoom = new ChatRoom((document.getElementById("chatRoomId") as HTMLInputElement).value);
ko.applyBindings(chatRoom, document.getElementById("chatBox"));

connection.on("ReceiveMessage", function (user: string, message: string) {
    chatRoom.chatMessages.push(new ChatMessage(user, message));
});

(async () => {
    try {
        await connection.start();
        await connection.invoke("joinChatRoom", chatRoom.chatRoomId);
        chatRoom.enabled(true);
    } catch (e) {
        console.error(e);
    }
})();

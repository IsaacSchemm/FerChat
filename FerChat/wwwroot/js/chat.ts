///<reference path="../lib/knockout/build/types/knockout.d.ts" />

declare var signalR: any;

interface ChatMessage {
    id: string;
    textContent: string;
    timestamp: string;
    user: {
        id: string;
        name: string;
    }
}

class ChatRoom {
    readonly enabled = ko.observable(false);
    readonly chatMessages = ko.observableArray<ChatMessage>();
    readonly textEntry = ko.observable("");

    constructor(readonly chatRoomId: string) { }

    async load() {
        const messages: ChatMessage[] = await fetch(`/Chat/Messages?chatRoomId=${this.chatRoomId}`).then(x => x.json());
        this.chatMessages(messages);
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

var chatRoom = new ChatRoom(/[&\?]chatRoomId=([0-9A-Fa-f\-]+)/.exec(location.search)![1]);
ko.applyBindings(chatRoom, document.getElementById("chatBox"));

connection.on("ReceiveMessage", function (message: ChatMessage) {
    chatRoom.chatMessages.push(message);
});

(async () => {
    try {
        await chatRoom.load();
    } catch (e) {
        console.error(e);
    }

    try {
        await connection.start();
        await connection.invoke("joinChatRoom", chatRoom.chatRoomId);
        chatRoom.enabled(true);
    } catch (e) {
        console.error(e);
    }
})();

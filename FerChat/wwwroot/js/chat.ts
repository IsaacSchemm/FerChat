declare var signalR: any;

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
(document.getElementById("sendButton") as HTMLButtonElement).disabled = true;

var chatRoomId = (document.getElementById("chatRoomId") as HTMLInputElement).value;

connection.on("ReceiveMessage", function (user, message) {
    var table = document.querySelector("#chatBox .main table");
    var tr = document.createElement("tr");
    table.appendChild(tr);
    var td = document.createElement("td");
    tr.appendChild(td);
    var name = document.createElement("div");
    name.className = "name";
    name.innerText = user;
    td.appendChild(name);
    var content = document.createElement("div");
    content.className = "content";
    content.innerText = message;
    td.appendChild(content);
});

connection.start().then(function () {
    connection.invoke("joinChatRoom", chatRoomId).then(function () {
        (document.getElementById("sendButton") as HTMLButtonElement).disabled = false;
    }).catch (function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = (document.getElementById("messageInput") as HTMLInputElement).value;
    connection.invoke("SendMessage", chatRoomId, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

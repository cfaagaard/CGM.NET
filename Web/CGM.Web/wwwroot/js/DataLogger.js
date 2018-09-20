"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/dataloggerhub").build();

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    li.textContent = message;
    var list = document.getElementById("messagesList");
    if (list.children.length===0) {
        list.appendChild(li);
    }
    else {
        list.insertBefore(li, list.children[0]);
    }

    if (list.children.length>10) {
        list.removeChild(list.children[10]);
    }
    
});

connection.on("ReceiveStatus", function (message) {
    document.getElementById("statusText").textContent=message;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});



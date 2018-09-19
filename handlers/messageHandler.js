const fs = require('fs');
exports.initMessageHandler = function(bot) {
    var filess = [];
    var commands = new Map();
    var embedMap = new Map();
    var dir = __dirname + "/../commands";

    var files = fs.readdirSync(dir);
    for(var i in files){
        if (!files.hasOwnProperty(i)) continue;
        var name = dir+'/'+files[i];
        //var name2 = files[i];
        if (!fs.statSync(name).isDirectory()){
            filess.push(name);
        }
    }
    
    files.forEach(function (v){
        const tmp = require('./../commands/'+v);
        tmp.alias.forEach(function(x){
            commands.set(x, tmp.command);
            embedMap.set(x, tmp.embed);
        })
    });

    bot.on("messageCreate", async (msg) => {
        if(msg.content.charAt(0) == "&") {
             var args = msg.content.split(" ");
             args[0] = args[0].substring(1, args[0].length);
            if(commands.get(args[0]) != undefined) {
                if(!embedMap.get(args[0])) {
                    var returnMessage = await commands.get(args[0])(args, msg);
                    bot.createMessage(msg.channel.id, returnMessage);
                } else {
                    var returnMessage = await commands.get(args[0])(args, msg);
                    returnMessage.send();
                }
            }
        }
    });
}
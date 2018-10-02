const ppHandler = require('./../handlers/PPHandler.js');
exports.alias = ["grantpp"];
exports.embed = false;
exports.command = async function(args, msg) {
    var mem = await msg.channel.guild.getRESTMember(msg.author.id);
    if(args.length != 3 && (mem.roles.includes("482015993959415808") || mem.roles.includes("491350887868923904") || msg.author.id == "213627387206828032")) {
        return "Usage: &grantpp @user PP";
    } else if(!mem.roles.includes("482015993959415808") && msg.author.id != "213627387206828032") {
        return "You do not have access to this command.";
    } else {
        await ppHandler.addPP(msg.mentions[0], args[2]);
        return "Granted " + args[1] + " " + args[2] + " PP";
    }
}
const ageHandler = require('./../handlers/ageHandler.js');
exports.alias = ["kill"];
exports.embed = false;
exports.command = async function(args, msg) {
    var mem = await msg.channel.guild.getRESTMember(msg.author.id);
    if(args.size < 2 && mem.roles.includes("482015993959415808")) {
        return "Please specify a player.";
    } else if (!mem.roles.includes("482015993959415808")) {
        return "You do not have access to this command."
    } else {
        var mem2 = await msg.channel.guild.getRESTMember(msg.mentions[0].id);
        ageHandler.kill(mem2);
        return "Killed  " + msg.mentions[0].username;
    }
    return "WIP";
}
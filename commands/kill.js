const ageHandler = require('./../handlers/ageHandler.js');
exports.alias = ["kill"];
exports.embed = false;
exports.command = async function(args, msg) {
    var mem = await msg.channel.guild.getRESTMember(msg.author.id);
    if(args.size < 2 && (mem.roles.includes("499949303708516374") || msg.author.id == "213627387206828032")) {
        return "Please specify a player.";
    } else if (!(mem.roles.includes("499949303708516374") || msg.author.id == "213627387206828032")) {
        return "You do not have access to this command."
    } else {
        ageHandler.kill(msg.mentions[0], msg.channel.guild);
        return "Killed  " + msg.mentions[0].username;
    }
    return "WIP";
}
const itemHandler = require('./../handlers/itemHandler.js');
exports.alias = ["grantitem"];
exports.embed = false;
exports.command = async function(args, msg) {
    var mem = await msg.channel.guild.getRESTMember(msg.author.id);
    if(args.length < 3 && (mem.roles.includes("482015993959415808") || msg.author.id == "213627387206828032")) {
        return "Usage: &grantitem @user ItemName";
    } else if(!mem.roles.includes("482015993959415808") && msg.author.id != "213627387206828032") {
        return "You do not have access to this command.";
    } else {
        if(args[2]+" "+args[3] == "Mining Ship") {
            itemHandler.addMiner(msg.mentions[0], 1);
            return "Added Mining Ship to " + args[1];
        } else if (args[2] == "Factory") {
            itemHandler.addFactory(mentions[0], 1);
            return "Added Factory to " + args[1];
        } else {
            return "Unknown Item";
        }
    }
}
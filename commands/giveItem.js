const itemHandler = require('./../handlers/itemHandler.js'); //Import item handler
exports.alias = ["grantitem"]; //Alias
exports.embed = false; //Is not embed
exports.command = async function(args, msg) { //Defines command
    var mem = await msg.channel.guild.getRESTMember(msg.author.id); //Get member
    if(args.length < 3 && (mem.roles.includes("499949303708516374") || msg.author.id == "213627387206828032")) { //Same authentication ifs as advance
        return "Usage: &grantitem @user ItemName"; 
    } else if(!mem.roles.includes("499949303708516374") && msg.author.id != "213627387206828032") {
        return "You do not have access to this command.";
    } else {
        if(args[2]+" "+args[3] == "Mining Ship") {
            itemHandler.addMiner(msg.mentions[0], 1); //Adds a miner if mining ship is in args
            return "Added Mining Ship to " + args[1];
        } else if (args[2] == "Factory") {
            itemHandler.addFactory(mentions[0], 1); //Adds factory if factory is in args
            return "Added Factory to " + args[1];
        } else {
            return "Unknown Item"; //Other wise reply with unknown item
        }
    }
}
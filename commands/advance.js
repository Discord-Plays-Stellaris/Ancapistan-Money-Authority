const ageHandler = require('./../handlers/ageHandler.js');
const wealthHandler = require('./../handlers/wealthHandler.js');
exports.alias = ["advance"];
exports.embed = false;
exports.command = async function(args, msg) {
    var mem = await msg.channel.guild.getRESTMember(msg.author.id);
    if(args.size < 2 && (mem.roles.includes("499949303708516374") || msg.author.id == "213627387206828032")) {
        return "Please specify the amount of years to advance after the command.";
    } else if(!mem.roles.includes("499949303708516374") && msg.author.id != "213627387206828032") {
        return "You do not have access to this command.";
    } else {
        await ageHandler.setAgeAll(msg.channel.guild, args[1]);
        await wealthHandler.payday(msg.channel.guild, args[1]);
        return "Advanced the time by " + args[1];
    } /*else {
        return "Towergame can't do if statements correctly lol";
    }*/
    //if(args.size >= 2 && (msg.channel.guild.getRESTMember(msg.author.id).roles.includes("482015993959415808") || msg.author.id == "213627387206828032"))
}
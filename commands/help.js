//Help command
exports.alias = ["help"];
exports.embed = false;
exports.command = async function(args, msg) {
    var channel = await msg.author.getDMChannel();
    var Embed = channel.createEmbed() //Creates the embed
    .title("Help") //Gives it a title
    .description("Current commands") //Gives it a description
    .field("Balance", "Showcases current age and amount of PP", false)
    .field("Ping", "Just a ping command", false)
    .color("#FF00FF") //Gives the embed a nice purple colour
    .image(msg.author.avatarURL) //Adds an image of the users avatar
    .footer("Brought to you by Ohcitrade"); //A nice flavour text as a footer
    var mem = await msg.channel.guild.getRESTMember(msg.author.id); //Get member
    if(mem.roles.includes("499949303708516374")) { //Same authentication ifs as advance
        Embed.field("grantPP", "Give political power", false)
        .field("kill", "Kill a player", false)
        .field("setPP", "Sets a players political power value", false)
        .field("setAge", "Sets a players age", false)
        .field("grantPP", "Grants political power");
    }
    if(msg.author.id == "213627387206828032") {
        Embed.field("eval", "Eval thing", false);
    }
    Embed.send();
    return "Info has been DM'd to you";
}
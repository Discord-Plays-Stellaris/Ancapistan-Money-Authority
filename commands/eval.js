exports.alias = ["eval"];
exports.embed = false;
exports.command = async function(args, msg) {
    if(msg.author.id != "213627387206828032") {
        return "You do not have the permission to use eval commands";
    } else {
        args.shift();
        var evalo = args.join(" ");
        return await eval(evalo);
    }
}
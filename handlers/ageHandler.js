var r = require('rethinkdbdash')();
exports.getAge = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
        expectancy: Math.floor(Math.random() * Math.floor(120 - 80 + 1) + 80)}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var age;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        age = obj.age;
    }).error(null);
    return age;
}
exports.setAgeAll = async function(guild, amount) {
    guild.members.forEach(async function (member) {
        await r.db('wealth').table('users').get(member.id).run().then( async function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(obj == null) { //if it doesn't exist, make an entry for the user
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(120 - 80 + 1) + 80)}, {conflict: "update"}).run();
            } else if (obj.age == null) {
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(120 - 80 + 1) + 80)}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var age;
        await r.db('wealth').table('users').get(member.id).run().then(async function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            age = obj.age;
            if(age >= obj.expectancy) {
                member.user.getDMChannel().createMessage("Death is coming, you have 24 hours(or the next bot restart) to seal any loose ends before your character gives up the ghost.");
                setTimeout(function() { kill(member) }, 86400000)
                await r.db('wealth').table('timers').insert([{id: user.id, member: member}]).run();
            }
            var newage = parseInt(age) + parseInt(amount);
            await r.db('wealth').table('users').get(member.id).update({age: newage}).run();
            });
        });
}
exports.fix = async function(guild) {
    guild.members.forEach(async function (member) {
        await r.db('wealth').table('users').get(member.id).run().then( async function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(obj == null) { //if it doesn't exist, make an entry for the user
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(120 - 80 + 1) + 80)}, {conflict: "update"}).run();
            } else if (obj.age == null) {
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(120 - 80 + 1) + 80)}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var age;
        await r.db('wealth').table('users').get(member.id).run().then(async function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            age = obj.age;
            var newage = age.substring(0,2);
            await r.db('wealth').table('users').get(member.id).update({age: newage}).run();
            });
        });
}
exports.kill = function(member) {
    var options = new Object();
    options.roles = [""];
    options.nick = member.user.username;
    member.edit(options);
    r.db('wealth').table('timers').get(member.user.id).delete().run();
}
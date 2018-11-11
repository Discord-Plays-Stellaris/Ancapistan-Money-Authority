var r = require('rethinkdbdash')();
var ppHandler = require('./PPHandler.js');
exports.getAge = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
        expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
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
exports.setAge = async function(user, age) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
        expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    await r.db('wealth').table('users').get(user.id).update({age: age});
}
exports.setAgeAll = async function(guild, amount) {
    guild.members.forEach(async function (member) {
        await r.db('wealth').table('users').get(member.id).run().then( async function(result) {
            json = JSON.stringify(result, null, 2);
            obj = JSON.parse(json);
            if(obj == null) { //if it doesn't exist, make an entry for the user
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
            } else if (obj.age == null) {
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var age;
        await r.db('wealth').table('users').get(member.id).run().then(async function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            age = obj.age;
            if(age >= obj.expectancy) {
                var chan = await member.user.getDMChannel();
                chan.createMessage("Death is coming, you have 24 hours(or the next bot restart) to seal any loose ends before your character gives up the ghost.");
                setTimeout(function() { kill(member.user.id, guild) }, 86400000)
                await r.db('wealth').table('timers').insert([{id: member.user.id}]).run();
            }
            var PP = await ppHandler.getPP(member.user);
            var newPP = Math.floor(PP-((PP/100)*age));
            await ppHandler.setPP(member.user, newPP);
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
                expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
            } else if (obj.age == null) {
                await r.db('wealth').table('users').insert({id: member.id,
                    age: Math.floor(Math.random() * Math.floor(25 - 20 + 1) + 20),
                expectancy: Math.floor(Math.random() * Math.floor(130 - 90 + 1) + 90)}, {conflict: "update"}).run();
            }
        }).error(null); //attempt to get the user wealth data
        var age;
        await r.db('wealth').table('users').get(member.id).run().then(async function(results) {
            json = JSON.stringify(results, null, 2);
            obj = JSON.parse(json);
            age = obj.expectancy;
            var newage = toInt(expectancy) + 10;
            await r.db('wealth').table('users').get(member.id).update({expectancy: newage}).run();
            });
        });
}
exports.kill = async function(user, guild) {
    var mem = await guild.getRESTMember(user.id);
    var roles = mem.roles;
    var roles = roles.filter(checkRole);
    var channel = await user.getDMChannel();
    channel.createMessage("Time's up, you have died.")
    guild.editMember(user.id, [{nick: mem.username, roles: roles}]);
    r.db('wealth').table('timers').get(user.id).delete().run();
    r.db('wealth').table('users').get(user.id).delete().run();
}

function checkRole(role) {
    var tmp = ["482015993959415808", "482689813133131786", "486389349064114189", "487536677431279627", "483367701130117140", "484361054089117697","490528648542158851","490534812436922368","492929538603614211","485073923726245911","487629626131218432","482689643658215475","492127503368978432","492360510491066373","491350887868923904","486942405636128779","490562112733577236","494771767353671691","494771930277347338","492333564826877953","490647916760006666","484570650350977044", "492673465283772420"]
    if(tmp.includes(role)) {
        return true;
    } else {
        return false;
    }
}
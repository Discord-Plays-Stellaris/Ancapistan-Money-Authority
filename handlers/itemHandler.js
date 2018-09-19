var r = require('rethinkdbdash')();
exports.getItems = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        var tmp = [];
        await r.db('wealth').table('users').insert({id: user.id,
            items: tmp}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var items;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        items = obj.items;
    }).error(null);
    return items;
}
exports.addMiner = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            items: {}}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var items;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        items = obj.items;
        var tmp = new Object();
        tmp.name = "Mining Ship";
        tmp.upkeep = "25000";
        tmp.upkeepResource = "nEC";
        tmp.produce = "333"
        tmp.produceResource = "Materials"
        items[items.length] = tmp;
        await r.db('wealth').table('users').get(member.id).update({items: items}).run();
    }).error(null);
}
exports.addFactory = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(async function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(obj.age == null || obj.age == undefined) { //if it doesn't exist, make an entry for the user
        await r.db('wealth').table('users').insert({id: user.id,
            items: {}}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var items;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        items = obj.items;
        var tmp = new Object();
        tmp.name = "Factory";
        tmp.upkeep = "25000";
        tmp.upkeepResource = "nEC";
        tmp.produce = "333"
        tmp.produceResource = "Materials"
        items[items.length] = tmp;
        await r.db('wealth').table('users').get(member.id).update({items: items}).run();
    }).error(null);
}
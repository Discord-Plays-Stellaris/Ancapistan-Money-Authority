var r = require('rethinkdbdash')();
exports.getPP = async function(user) {
    await r.db('wealth').table('users').get(user.id).run().then(function(result) {
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            pp: 0}, {conflict: "update"}).run();
        } else if (obj.pp == null) {
            r.db('wealth').table('users').insert({id: user.id,
                pp: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var pp;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        pp = obj.pp;
    }).error(null);
    return pp.toString();
}
exports.addPP = async function(user, amount) {
    await r.db('wealth').table('users').get(user.id).run().then(function(result) {
        json = JSON.stringify(result, null, 2);
        obj = JSON.parse(json);
        if(result == null) { //if it doesn't exist, make an entry for the user
            r.db('wealth').table('users').insert({id: user.id,
            pp: 0}, {conflict: "update"}).run();
        } else if (obj.pp == null) {
            r.db('wealth').table('users').insert({id: user.id,
                pp: 0}, {conflict: "update"}).run();
        }
    }).error(null); //attempt to get the user wealth data
    var pp;
    await r.db('wealth').table('users').get(user.id).run().then(function(results) {
        json = JSON.stringify(results, null, 2);
        obj = JSON.parse(json);
        pp = obj.pp;
    }).error(null);
    r.db('wealth').table('users').get(user.id).update({pp: parseInt(pp) + parseInt(amount)}).run();
}
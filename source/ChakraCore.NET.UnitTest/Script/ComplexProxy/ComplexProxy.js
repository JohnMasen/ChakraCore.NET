function MultiTransfer() {
    var result = "";
    for (var i = 0; i < proxies.length; i++) {
        result = result  +proxies[i].GetName()+",";
    }
    return result;
}
mergeInto(LibraryManager.library, {

    ConnectToWallet: function(){
        TryConnectToWallet();
    },

    DisconnectWallet: function(){
        Disconnect();
    },

    GetAllNfts: function(){
        return getAllNftData()

    }
});
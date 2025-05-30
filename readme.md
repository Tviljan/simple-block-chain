# Simple Blockchain in C#

This is a simple implementation of a blockchain in C# with proof-of-work mining. It provides a command-line interface for interacting with the blockchain.

## Commands

- `add [data]`: Adds a new block to the blockchain with the specified data. The block will be mined according to the current difficulty.
- `validate`: Checks if the blockchain is valid and prints the result.
- `print`: Displays all blocks in the blockchain with their details.
- `exit`: Stops the execution of the program.
- `save`: Saves the current state of the blockchain to a file named "blockchain.txt".
- `load`: Loads the blockchain state from a file named "blockchain.txt".

## Usage

Run the program and enter commands at the prompt. For example:

> add Hello, blockchain! 

The system will mine the block with the current difficulty level and add it to the blockchain.

## Block Structure

Each block in the blockchain has the following properties:

- `Index`: The position of the block in the chain.
- `TimeStamp`: The time when the block was created.
- `PreviousHash`: The hash of the previous block in the chain.
- `Hash`: The hash of the current block.
- `Data`: The data stored in the block.
- `Nonce`: A number used in mining to find a hash with the required difficulty.

## Mining and Difficulty Adjustment

The blockchain implements a proof-of-work system where:

- Each block must be mined by finding a hash that starts with a certain number of zeros.
- The number of required zeros is the current "difficulty" level.
- The system automatically adjusts difficulty every 5 blocks.
- The target block mining time is 10 seconds.
- If blocks are being mined too quickly, difficulty increases.
- If blocks are being mined too slowly, difficulty decreases.

## Disclaimer

This is a simple implementation for educational purposes and should not be used for real-world applications.
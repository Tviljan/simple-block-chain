# Simple Blockchain in C#

This is a simple implementation of a blockchain in C#. It provides a command-line interface for interacting with the blockchain.

## Commands

- `add [data]`: Adds a new block to the blockchain with the specified data.
- `validate`: Checks if the blockchain is valid and prints the result.
- `exit`: Stops the execution of the program.
- `save`: Saves the current state of the blockchain to a file named "blockchain.txt".
- `load`: Loads the blockchain state from a file named "blockchain.txt".

## Usage

Run the program and enter commands at the prompt. For example:

> add Hello, blockchain! 


## Block Structure

Each block in the blockchain has the following properties:

- `Index`: The position of the block in the chain.
- `TimeStamp`: The time when the block was created.
- `PreviousHash`: The hash of the previous block in the chain.
- `Hash`: The hash of the current block.

## Disclaimer

This is a simple implementation for educational purposes and should not be used for real-world applications.
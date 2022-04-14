#!node

import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import shell from 'shelljs';
import chalk from 'chalk';
import concurrently from 'concurrently';
import { execa } from 'execa';

const info = chalk.blueBright
const warn = chalk.yellow
const error = chalk.red
const success = chalk.green
const log = console.log

// Crash script on error
shell.config.fatal = true;

async function cleanHandler() {
    log(info("Cleaning..."));

    shell.rm("-rf", "./Demo/**/*.fs.js");
    shell.rm("-rf", "./Demo/**/*.fs.js.map");
    shell.rm("-rf", "./Demo/bin");
    shell.rm("-rf", "./Demo/obj");

    shell.rm("-rf", "./Sources/**/*.fs.js");
    shell.rm("-rf", "./Sources/**/*.fs.js.map");
    shell.rm("-rf", "./Sources/bin");
    shell.rm("-rf", "./Sources/obj");

    log(success("Cleaned!"));
}

async function simpleExec (cmd, cwd) {
    return await execa(
        cmd,
        {
            stdio: 'inherit',
            shell: true,
            cwd: cwd
        }
    )
}

async function devHandler() {
    await cleanHandler();

    concurrently([
        {
            command: "npx vite dev",
            cwd: "./Demo"
        },
        {
            command: "dotnet fable --watch",
            cwd: "./Demo"
        }
    ],
    {
        prefix: "none"
    })
}

async function releaseHandler () {
    await cleanHandler();

    log(info("Building..."));

    await simpleExec("dotnet build", ".");
    await simpleExec(
        'dotnet release nuget' +
        '--project ./Sources./Glutinum.Feliz.ReactResizeDetector.fsproj' +
        '--source nuget.org' +
        '--api-key %nuget_key%'
    )
}

yargs(hideBin(process.argv))
    .completion()
    .strict()
    .help()
    .alias("help", "h")
    .command(
        "clean",
        "Clean build artifacts",
        () => {},
        cleanHandler
    )
    .command(
        "dev",
        "Run dev server for the demo",
        () => {},
        devHandler
    )
    .command(
        "release",
        "Build and release a new version",
        () => {},
        releaseHandler
    )
    .version(false)
    .argv

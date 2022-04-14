#!node

import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';
import shell from 'shelljs';
import chalk from 'chalk';
import concurrently from 'concurrently';
import { execa } from 'execa';
import simpleGit from 'simple-git';
import fs from 'fs';
import path from 'path';
import parseChangelog from "changelog-parser";

const info = chalk.blueBright
const success = chalk.green
const log = console.log

// Crash script on error
shell.config.fatal = true;

export const getLastVersion = async () => {

    // checks if the package.json and CHANGELOG exist
    const changelogPath = path.resolve("CHANGELOG.md")

    if (!fs.existsSync(changelogPath)) {
        log(chalk.red(`CHANGELOG.md not found`))
    }

    // read files content
    const changelogContent = fs.readFileSync(changelogPath).toString().replace("\r\n", "\n")

    const changelog = await parseChangelog({ text: changelogContent })

    let versionInfo = undefined;

    // Find the first version which is not Unreleased
    for (const version of changelog.versions) {
        if (version.title.toLowerCase() !== "unreleased") {
            versionInfo = version;
            break;
        }
    }

    if (versionInfo === undefined) {
        log(chalk.red(`No version ready to be released found in the CHANGELOG.md`))
        process.exit(1)
    }

    return versionInfo;
}

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
    const status = await simpleGit().status();

    // Get all the uncommitted changes
    // We make an exception for the CHANGELOG.md file
    const uncommittedFiles =
        status.files.filter((file) => {
            return file.path !== "CHANGELOG.md"
        });

    if (uncommittedFiles.length > 0) {
        log(error("You have uncommitted changes. Please commit your changes before deploying."))
        return;
    }

    await cleanHandler();

    log(info("Building..."));

    await simpleExec("dotnet build", ".");
    await simpleExec(
        'dotnet release nuget' +
        '--project ./Sources./Glutinum.Feliz.ReactResizeDetector.fsproj' +
        '--source nuget.org' +
        '--api-key %nuget_key%'
    )

    const versionInfo  = await getLastVersion();

    await simpleGit()
        // Add the changes done to the files that can be updated
        // during the release process
        .add("CHANGELOG.md")
        .commit(`Release ${versionInfo.version}`)
        // Create a new tag for the release
        .addAnnotatedTag(`v${versionInfo.version}`, `Release ${versionInfo.version}`)
        // Push the changes to the remote
        .push()
        // Push the tag to the remote
        .push("origin", `${versionInfo.version}`)
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

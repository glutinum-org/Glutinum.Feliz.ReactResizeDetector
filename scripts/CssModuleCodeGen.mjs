import fg from 'fast-glob';
import fsSync from 'node:fs';
import fs from 'node:fs/promises';

// This is a very naive code generator to generate the
// bindings for CSS modules.
// This should become a Vite JS plugin in the future, to make it
// more robust and performant.

const entries = await fg('./Client/src/**/*.module.scss');

const destination = "./Client/src/CssModules.fs";
// This is a very naive way to detect CSS classes.
// In the future, it would be better to hook into Vite js and PostCSS
// but for now let's do it the stupid way. As it should work for most cases.
const cssClassRegex = /(\s)*\.(?<className>([\w-])+)(\s)*\{/gm;

const hyphenToCamelCase = (str) => {
    return str.replace(/[-_](\w)/g, (match, p1) => p1.toUpperCase());
}

const output = fsSync.createWriteStream(destination);

const writeToDestination = (str) => {
    return new Promise((resolve, reject) => {
        output.write(str, (err) => {
            if (err) {
                reject(err);
            } else {
                resolve();
            }
        });
    });
}

await writeToDestination(`module CssModules

// IMPORTANT: Do not edit this file
// It is generated each time a CSS module is updated.

open Fable.Core
open Feliz`);

for (const entry of entries) {
    const relativeFileName = entry.replace('./Client/src/', '');

    const content = await fs.readFile(entry, 'utf8');

    let m;
    let classNames = [];

    while ((m = cssClassRegex.exec(content)) !== null) {
        classNames.push(m.groups.className);
    }

    const properties =
        classNames
            // Keep only unique class name
            .filter((v, i, a) => a.indexOf(v) === i)
            .map(className => {
                const camelCasePropertyName = hyphenToCamelCase(className);

                return `
    [<Emit("$0[\\"${className}\\"]")>]
    abstract ${camelCasePropertyName} : string`
                }
            )
            .join('\n');

    if (properties.length == 0) {
        continue;
    }

    const moduleName =
        relativeFileName
            .replace(".module.scss", "")
            .replace("/", "_")
            .replace(".", "_");

    const gen =
        `

/// Definition of the ${relativeFileName} CSS module.
type ${moduleName} =
        ${properties}
        `.trimEnd();

    await writeToDestination(gen);
}
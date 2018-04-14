/*---------------------------------------------------------
 * Copyright (C) Microsoft Corporation. All rights reserved.
 *--------------------------------------------------------*/
'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
/*
 * Set the following compile time flag to true if the
 * debug adapter should run inside the extension host.
 * Please note: the test suite does no longer work in this mode.
 */
const EMBED_DEBUG_ADAPTER = false;
function activate(context) {
    // context.subscriptions.push(vscode.commands.registerCommand('extension.mock-debug.getProgramName', config => {
    // 	return vscode.window.showInputBox({
    // 		placeHolder: "Please enter the name of a markdown file in the workspace folder",
    // 		value: "readme.md"
    // 	});
    // }));
    // register a configuration provider for 'mock' debug type
    const provider = new MockConfigurationProvider();
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider('ccn', provider));
    context.subscriptions.push(provider);
}
exports.activate = activate;
function deactivate() {
    // nothing to do
}
exports.deactivate = deactivate;
class MockConfigurationProvider {
    /**
     * Massage a debug configuration just before a debug session is being launched,
     * e.g. add all missing attributes to the debug configuration.
     */
    resolveDebugConfiguration(folder, config, token) {
        // if launch.json is missing or empty
        if (!config.type && !config.request && !config.name) {
            const editor = vscode.window.activeTextEditor;
            if (editor && editor.document.languageId === 'markdown') {
                config.type = 'chakracore.net-debug';
                config.name = 'Launch';
                config.request = 'launch';
                config.program = '${file}';
                config.pauseOnLaunch = true;
            }
        }
        // if (!config.program) {
        // 	return vscode.window.showInformationMessage("Cannot find a program to debug").then(_ => {
        // 		return undefined;	// abort launch
        // 	});
        // }
        if (EMBED_DEBUG_ADAPTER) {
            // start port listener on launch of first debug session
            // if (!this._server) {
            // 	// start listening on a random port
            // 	this._server = Net.createServer(socket => {
            // 		const session = new MockDebugSession();
            // 		session.setRunAsServer(true);
            // 		session.start(<NodeJS.ReadableStream>socket, socket);
            // 	}).listen(0);
            // }
            // make VS Code connect to debug server instead of launching debug adapter
            // config.debugServer = this._server.address().port;
        }
        return config;
    }
    dispose() {
        if (this._server) {
            this._server.close();
        }
    }
}
//# sourceMappingURL=extension.js.map
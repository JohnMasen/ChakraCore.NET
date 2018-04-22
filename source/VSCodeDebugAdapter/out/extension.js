/*---------------------------------------------------------
 * Copyright (C) Microsoft Corporation. All rights reserved.
 *--------------------------------------------------------*/
'use strict';
Object.defineProperty(exports, "__esModule", { value: true });
const vscode = require("vscode");
const path = require("path");
/*
 * Set the following compile time flag to true if the
 * debug adapter should run inside the extension host.
 * Please note: the test suite does no longer work in this mode.
 */
// const EMBED_DEBUG_ADAPTER = false;
function activate(context) {
    // register a configuration provider for 'ccn' debug type
    const provider = new CCNConfigurationProvider();
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider('ccn', provider));
    context.subscriptions.push(provider);
}
exports.activate = activate;
function deactivate() {
    // nothing to do
}
exports.deactivate = deactivate;
class CCNConfigurationProvider {
    /**
     * Massage a debug configuration just before a debug session is being launched,
     * e.g. add all missing attributes to the debug configuration.
     */
    resolveDebugConfiguration(folder, config, token) {
        // if launch.json is missing or empty
        if (!config.type && !config.request && !config.name) {
            const editor = vscode.window.activeTextEditor;
            if (editor && editor.document.languageId === 'javascript') {
                config.type = 'ccn';
                config.name = 'ccn Launch';
                config.request = 'launch';
                config.runAsServer = false;
                config.port = 3515;
                config.pauseOnLaunch = false;
            }
        }
        if ((config.runAsServer) && config.runAsServer == true) {
            this._port = config.serverPort;
        }
        else {
            config.debugServer = config.port;
        }
        //TODO: add auto source mapping feature
        return config;
    }
    debugAdapterExecutable(folder, token) {
        let v = vscode.extensions.getExtension("JohnMasen.chakracorenet-debug");
        if (!v) {
            return null;
        }
        const p = path.join(v.extensionPath, "./out/DebugAdapter.js");
        const port = this._port || "3515";
        let result = { command: "node", args: [p, port] };
        return result;
    }
    dispose() {
    }
}
//# sourceMappingURL=extension.js.map
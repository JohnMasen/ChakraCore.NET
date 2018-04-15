/*---------------------------------------------------------
 * Copyright (C) Microsoft Corporation. All rights reserved.
 *--------------------------------------------------------*/

'use strict';

import * as vscode from 'vscode';
import { WorkspaceFolder, DebugConfiguration, ProviderResult, CancellationToken } from 'vscode';
import * as path  from 'path';
import * as Net from 'net';

/*
 * Set the following compile time flag to true if the
 * debug adapter should run inside the extension host.
 * Please note: the test suite does no longer work in this mode.
 */
const EMBED_DEBUG_ADAPTER = false;

export function activate(context: vscode.ExtensionContext) {

	// context.subscriptions.push(vscode.commands.registerCommand('extension.mock-debug.getProgramName', config => {
	// 	return vscode.window.showInputBox({
	// 		placeHolder: "Please enter the name of a markdown file in the workspace folder",
	// 		value: "readme.md"
	// 	});
	// }));

	// register a configuration provider for 'mock' debug type
	const provider = new MockConfigurationProvider()
	context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider('ccn', provider));
	context.subscriptions.push(provider);
}

export function deactivate() {
	// nothing to do
}
export interface DebugAdapterExecutable {

	/**

	 * The command path of the debug adapter executable.

	 * A command must be either an absolute path or the name of an executable looked up via the PATH environment variable.

	 * The special value 'node' will be mapped to VS Code's built-in node runtime.

	 */

	readonly command: string;



	/**

	 * Optional arguments passed to the debug adapter executable.

	 */

	readonly args: string[];


}
class MockConfigurationProvider implements vscode.DebugConfigurationProvider {

	private _server?: Net.Server;
	private _port?:string;
	/**
	 * Massage a debug configuration just before a debug session is being launched,
	 * e.g. add all missing attributes to the debug configuration.
	 */
	resolveDebugConfiguration(folder: WorkspaceFolder | undefined, config: DebugConfiguration, token?: CancellationToken): ProviderResult<DebugConfiguration> {

		// if launch.json is missing or empty
		if (!config.type && !config.request && !config.name) {
			const editor = vscode.window.activeTextEditor;
			if (editor && editor.document.languageId === 'javascript' ) {
				config.type = 'chakracore.net-debug';
				config.name = 'Launch';
				config.request = 'launch';
				config.program = '${file}';
				config.pauseOnLaunch = true;
			}
		}
		this._port=config.serverPort;

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

	debugAdapterExecutable?(folder: WorkspaceFolder | undefined, token?: CancellationToken): ProviderResult<DebugAdapterExecutable>	{
		let v=vscode.extensions.getExtension("JohnMasen.chakracore.net-debug");
		if (!v) {
			return null;
		}
		const p=path.join(v.extensionPath,"./out/DebugAdapter.js");
		const port=this._port||"1234";
		let result:DebugAdapterExecutable={command:"node",args:[p,port] };
		return result;
	}
	dispose() {
		if (this._server) {
			this._server.close();
		}
	}
}

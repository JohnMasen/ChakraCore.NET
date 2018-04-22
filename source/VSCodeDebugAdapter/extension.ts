/*---------------------------------------------------------
 * Copyright (C) Microsoft Corporation. All rights reserved.
 *--------------------------------------------------------*/

'use strict';

import * as vscode from 'vscode';
import { WorkspaceFolder, DebugConfiguration, ProviderResult, CancellationToken } from 'vscode';
import * as path  from 'path';

/*
 * Set the following compile time flag to true if the
 * debug adapter should run inside the extension host.
 * Please note: the test suite does no longer work in this mode.
 */
// const EMBED_DEBUG_ADAPTER = false;

export function activate(context: vscode.ExtensionContext) {

	// register a configuration provider for 'ccn' debug type
	const provider = new CCNConfigurationProvider()
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
class CCNConfigurationProvider implements vscode.DebugConfigurationProvider {

	
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
				config.type = 'ccn';
				config.name = 'ccn Launch';
				config.request = 'launch';
				config.runAsServer=false;
				config.port=3515;
				config.pauseOnLaunch = false;
			}
		}
		
		if((config.runAsServer) && config.runAsServer==true){
			this._port=config.serverPort;
		}
		else{
			config.debugServer=config.port; 
		}
		//TODO: add auto source mapping feature
		

		return config;
	}

	debugAdapterExecutable?(folder: WorkspaceFolder | undefined, token?: CancellationToken): ProviderResult<DebugAdapterExecutable>	{
		let v=vscode.extensions.getExtension("JohnMasen.chakracorenet-debug");
		if (!v) {
			return null;
		}
		const p=path.join(v.extensionPath,"./out/DebugAdapter.js");
		const port=this._port||"3515";
		let result:DebugAdapterExecutable={command:"node",args:[p,port] };
		return result;
	}
	dispose() {
		
	}
}

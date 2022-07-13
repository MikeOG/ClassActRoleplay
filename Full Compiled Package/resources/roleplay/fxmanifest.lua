fx_version 'adamant'
game 'gta5'
resource_type 'gametype' {name = "ClassActRP"}

client_scripts {
	"client/*.net.dll",
	"NUICallbackHandler.lua",
 	'InitSharedData.lua',
 	"shared/**/**/*.lua",
 	"client/**/*.lua",
}
server_scripts {
	"server/*.net.dll",
	'InitSharedData.lua',
        "shared/**/**/*.lua",
        "server/**/*.lua",
}
files {
	"client/*.dll",
	"ui/js/materialize.js",
	"ui/js/login.js",
	"ui/js/eventfuncs.js",
	"ui/js/characterobject.js",
	"ui/css/materialize.css",
	"ui/index.html"
}
ui_page "ui/index.html"
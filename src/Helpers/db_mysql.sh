#!/bin/bash

user=$1
password=$2
dbName=$3
hostname=$4
port=$5
changescriptpath=$6

#get files from change scripts path
scriptsinfolder=($(ls $changescriptpath))

#function for execute script content
function execfunc {
	echo	`mysql --user="$user" --password="$password" --host="$hostname" --port="$port" --database="$dbName" --execute="$1"`
}

#function for insert into schemachangelog
function insertchangelog {
	currentdate="'$(date +'%d.%m.%Y')'"
	currenttime="'$(date +"%I:%M:%S")'"
	filename="'$1'"
	insertchangelogcommand='INSERT INTO SchemaChangeLog (ScriptName, ExecuteDate, ExecuteTime) VALUES('$filename', '$currentdate','$currenttime');'
        ($(execfunc "$insertchangelogcommand"))
}

initialscriptname='00000.Initial.sql';

#create db if not exists
if ! mysql --user="$user" --password="$password" --host="$hostname" --port="$port" --execute "use "$dbName""; then
	content=$(cat $changescriptpath/$initialscriptname)
	content="${content/'$(DatabaseName)'/$dbName}"    
	mysql --user="$user" --password="$password" --host="$hostname" --port="$port" --execute "$content";
	else
	echo "db $dbName alredy exists"
fi

#get executed scripts from db
executedscripts=($(execfunc 'SELECT ScriptName FROM SchemaChangeLog;'))

#remove column header from result
delete=ScriptName
executedscripts=("${executedscripts[@]/$delete}" )

#array diff between executed and new scripts
newscripts=(`echo ${scriptsinfolder[@]} ${executedscripts[@]} | tr ' ' '\n' | sort | uniq -u`)

doneMessage="OK";

for row in "${newscripts[@]}";do
#if file exists
	if [ -e $changescriptpath/$row ]
	then
#if file is initialscript -> continue
		if [ $row = $initialscriptname ]
		then
			continue
		fi
#apply changescriptcontent
		mysql --user="$user" --password="$password" --host="$hostname" --port="$port" --database="$dbName" < $changescriptpath/$row;
		if [ $? -eq 0 ];
		then
			($(insertchangelog "$row"))
		else
			doneMessage="error on execute script $row"
			break
		fi
	fi
done

echo $doneMessage;
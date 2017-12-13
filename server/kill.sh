ps -ef | grep skynet | grep -v grep | awk '{print $2}' | xargs kill -9

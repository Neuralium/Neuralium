#!/bin/bash

cd /home/jdb/work/Neuralia/neuralium/Neuralium/src

if scripts/publish-all.sh ; then
   scripts/copy-nodes.sh
    echo "build success"
else
    echo "build failed"
exit
fi

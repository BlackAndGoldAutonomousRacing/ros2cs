#!/bin/bash

display_usage() {
    echo "Usage: "
    echo "build.sh [--with-tests] [--standalone] [--debug]"
    echo ""
    echo "Options:"
    echo "--with-tests - build with tests."
    echo "--standalone - standalone version"
}

if [ -z "${ROS_DISTRO}" ]; then
    echo "Source your ros2 distro first (foxy, galactic, humble or rolling are supported)"
    exit 1
fi

TESTS=0
MSG="Build started."
STANDALONE=OFF
DEBUG=0

while [[ $# -gt 0 ]]; do
  key="$1"
  case $key in
    -t|--with-tests)
      TESTS=1
      MSG="$MSG (with tests)"
      shift # past argument
      ;;
    -s|--standalone)
      STANDALONE=ON
      MSG="$MSG (standalone)"
      shift # past argument
      ;;
    -h|--help)
      display_usage
      exit 0
      shift # past argument
      ;;
    -d|--debug)
      MSG="$MSG (debug)"
      DEBUG=1
      shift # past argument
      ;;
    *)    # unknown option
      shift # past argument
      ;;
  esac
done

echo $MSG
if [ $DEBUG == 1 ]
then
colcon build \
--merge-install \
--event-handlers console_direct+ \
--cmake-args \
-DCMAKE_BUILD_TYPE=Release \
-DSTANDALONE_BUILD=$STANDALONE \
-DBUILD_TESTING=$TESTS \
-DCMAKE_SHARED_LINKER_FLAGS="-Wl,-rpath,'\$ORIGIN',-rpath=.,--disable-new-dtags" \
--no-warn-unused-cli
else
colcon build \
--merge-install \
--cmake-args \
-DCMAKE_BUILD_TYPE=Release \
-DSTANDALONE_BUILD=$STANDALONE \
-DBUILD_TESTING=$TESTS \
-DCMAKE_SHARED_LINKER_FLAGS="-Wl,-rpath,'\$ORIGIN',-rpath=.,--disable-new-dtags" \
--no-warn-unused-cli
fi

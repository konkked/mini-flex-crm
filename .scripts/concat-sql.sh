#!/bin/bash

# Check if the correct number of arguments are provided
if [ "$#" -ne 2 ]; then
    echo "Usage: $0 <input_folder> <output_file>"
    exit 1
fi

input_folder=$1
output_file=$2

# Delete the output file if it already exists
if [ -f "$output_file" ]; then
    rm "$output_file"
fi

# Create or empty the output file
> "$output_file"

# Find and concatenate all .sql files
find "$input_folder" -type f -name "*.sql" | sort | while read -r file; do
    # Add a comment indicating the start of the file
    echo "-- Start of file: $file -------------------------"
    echo ""
    echo ""
    cat "$file"
    echo ""
    echo ""
    # Add a separator between files
    echo "-- End of file: $file-----------------------------"
    echo ""
    # Add a comment indicating the end of the file
done >> "$output_file"

echo "All .sql files from $input_folder have been concatenated into $output_file"
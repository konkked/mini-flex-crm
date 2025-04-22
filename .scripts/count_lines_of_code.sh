#!/bin/bash

# Directory to analyze (current directory by default)
DIR=${1:-.}

# Find all files, group by extension, and count files and lines
echo "File Count and Line Count by Extension in Directory: $DIR (excluding node_modules)"
echo "-------------------------------------------------------"
find "$DIR" -type d -name "node_modules" -prune -o -type f -print | while read -r file; do
    ext="${file##*.}" # Extract file extension
    if [[ "$file" == "$ext" ]]; then
        ext="no_extension" # Handle files without extensions
    fi
    lines=$(wc -l < "$file" 2>/dev/null || echo 0) # Count lines in the file
    echo "$ext $lines"
done | awk '
{
    count[$1]++
    total_lines[$1] += $2
}
END {
    printf "%-15s %-10s %-10s\n", "Extension", "Files", "Lines"
    printf "%-15s %-10s %-10s\n", "---------", "-----", "-----"
    for (ext in count) {
        printf "%-15s %-10d %-10d\n", ext, count[ext], total_lines[ext]
    }
}'
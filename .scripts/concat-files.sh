#!/bin/bash

# Check if the correct number of arguments are provided
if [ "$#" -lt 3 ]; then
    echo "Usage: $0 <ext> <input_folder> <output_file>"
    exit 1
fi

ext=$1
input_folder=$2
output_file=$3

# Function to determine comment style based on file extension
get_comment_chars() {
    case "$1" in
        # Hash-style comments
        sh|py|yaml|yml|toml|r|ini|tf|rb) echo "#" ;;
        # Double slash comments
        c|cpp|h|hpp|cs|java|js|ts|rs|go|swift|kt|scala) echo "//" ;;
        # SQL-style comments
        sql) echo "--" ;;
        # HTML/XML style comments
        html|xml|xhtml|xsd|svg|md) echo "<!-- -->" ;;
        # ASP/JSP/PHP-style comments
        jsp|php) echo "<%-- --%>" ;;
        # Lisp/Scheme comments
        lisp|clj|rkt|ss) echo ";" ;;
        # Tex/Matlab comments
        tex|m) echo "%" ;;
        # OCaml/ML-style comments
        ml|mli|sml) echo "(* *)" ;;
        # Pascal/Delphi-style comments
        pas|pp) echo "{ }" ;;
        # Lua-style comments
        lua) echo "-- --[[ ]]" ;;
        # Twig/Smarty-style comments
        twig|smarty) echo "{# #}" ;;
        # No comment style for text files
        txt|csv|log) echo "" ;;
        # Default fallback (treat as hash-style comment)
        *) echo "#" ;;
    esac
}

comment_chars=$(get_comment_chars "$ext")

# Delete the output file if it already exists
if [ -f "$output_file" ]; then
    rm "$output_file"
fi

# Create or empty the output file
> "$output_file"

# Determine if hidden files should be included
include_hidden=false
if [ "$4" == "--include-hidden" ]; then
    include_hidden=true
fi

# Process each file with the given extension
find "$input_folder" -type f -name "*.$ext" | sort | while read -r file; do
    if [ "$include_hidden" = false ] && [[ "$file" == */.*/* ]]; then
        echo "Skipping hidden file: $file" >&2
        continue
    fi
    echo "Processing file: $file" >&2
    if [[ "$file" == *"$output_file"* ]]; then
        echo "Skipping output file: $file" >&2
        continue
    fi

    # Add start comment if applicable
    if [ -n "$comment_chars" ]; then
        case "$comment_chars" in
            "#"|"--"|";"|"%"|"//")
                echo "$comment_chars Start of file: $file -------------------------" >> "$output_file"
                ;;
            "<!-- -->")
                echo "<!-- Start of file: $file ------------------------->" >> "$output_file"
                ;;
            "{ }")
                echo "{ Start of file: $file ------------------------- }" >> "$output_file"
                ;;
            "(* *)")
                echo "(* Start of file: $file ------------------------- *)" >> "$output_file"
                ;;
            "{# #}")
                echo "{# Start of file: $file ------------------------- #}" >> "$output_file"
                ;;
            "<%-- --%>")
                echo "<%-- Start of file: $file ------------------------- --%>" >> "$output_file"
                ;;
        esac
        echo "" >> "$output_file"
    fi

    cat "$file" >> "$output_file"
    echo "" >> "$output_file"

    # Add end comment if applicable
    if [ -n "$comment_chars" ]; then
        case "$comment_chars" in
            "#"|"--"|";"|"%"|"//")
                echo "$comment_chars End of file: $file ---------------------------" >> "$output_file"
                ;;
            "<!-- -->")
                echo "<!-- End of file: $file ------------------------->" >> "$output_file"
                ;;
            "{ }")
                echo "{ End of file: $file ------------------------- }" >> "$output_file"
                ;;
            "(* *)")
                echo "(* End of file: $file ------------------------- *)" >> "$output_file"
                ;;
            "{# #}")
                echo "{# End of file: $file ------------------------- #}" >> "$output_file"
                ;;
            "<%-- --%>")
                echo "<%-- End of file: $file ------------------------- --%>" >> "$output_file"
                ;;
        esac
        echo "" >> "$output_file"
    fi

done

echo "All *.$ext files from $input_folder have been concatenated into $output_file"

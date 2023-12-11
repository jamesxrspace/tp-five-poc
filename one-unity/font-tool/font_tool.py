#!/usr/bin/env python3
'''
    File name: font_tool.py
    Author: Bala Huang
    Date created: 8/8/2023
    Date last modified: 8/9/2023
    Python Version: 3.10

    Description: This script is used to get the sequence of Unicode (Hex) character ranges
        for a TrueType Font

    usage: font_tool.py [-h] -f FONT

    Options:
      -h, --help                    show this help message and exit
      -f FONT, --font FONT          The filename of the TrueType Font
'''

import argparse
from itertools import zip_longest
from fontTools.ttLib import TTFont


def get_unicode_character_sequence(codes):
    # pylint: disable=line-too-long
    '''
    The Python implementation of TMPro.EditorUtilities.TMP_EditorUtility.GetUnicodeCharacterSequence()
    returns a string containing a sequence of Unicode (Hex) character ranges.
    '''

    if not codes:
        return ''

    # Sort the character codes
    codes = sorted(codes)
    # Initialize the range list
    non_consecutive_codes = [r for r in zip_longest(codes, codes[1:]) if r[0] + 1 != r[1]]
    str_ranges = []
    min_code = codes[0]
    for (max_code, next_min_code) in non_consecutive_codes:
        code_str = f"{min_code:04X}" if min_code == max_code else f"{min_code:04X}-{max_code:04X}"
        str_ranges.append(code_str)
        min_code = next_min_code

    return ",".join(str_ranges)

def main():
    '''
    The main function
    '''

    # Create a parser
    parser = argparse.ArgumentParser()
    # Add argument options
    parser.add_argument("-f", "--font", required=True, help="The filename of the TrueType Font")
    # Parse arguments
    args = parser.parse_args()
    # Open the font file
    with TTFont(args.font) as font:
        # Get the character table
        cmap = font["cmap"]
        # Get all character codes
        codes = cmap.getBestCmap().keys()
        # Get Unicode hexadecimal range
        character_sequence = get_unicode_character_sequence(codes)
        # Display results on terminal
        print(character_sequence)

if __name__ == '__main__':
    main()

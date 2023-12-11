'''
Test font_tool.py
'''
import unittest
from fontTools.ttLib import TTFont
from font_tool import get_unicode_character_sequence

class TestFontTool(unittest.TestCase):
    '''
    Testcase for font_tool.py
    '''
    def setUp(self):
        '''
        setup for test
        '''
        self.font = TTFont("icon-xr.ttf")

    def tearDown(self):
        '''
        teardown for test
        '''
        self.font.close()

    def test_get_unicode_character_sequence(self):
        '''
        Test get_unicode_character_sequence()
        '''
        # pylint: disable=line-too-long
        valid_sequence = "0000-0001,0020-005F,0061-007E,00A3-00A5,00A7,00B0,00C1,00C4-00C6,00C9,00D3,00D5-00D6,00D8,00DE-00E1,00E4-00E5,00F3-00F6,00F8,0110,017D,0264,20A9,2295,22A0-22A1,2301,2339,2360,23F1,25A2-25A3,25B1,25B4,25B8,25C2,25C6,25C9,25CE,25D5,25DB,25DD-25DE,25E1,25F6-2603,2605-2611,2613-2614,2616-2617,261A-261C,261E-261F,2621,2629,2631-2635,2637,263A,263E,2643-2646,265A-265E,2660-2663,2665-2675,2680,268D,2692,26AC,26CB,2713,2721,2726-2728,274F,27BF,27F4,27FF,290C-290D,2934,2964,2970,2979,297F,2B14-2B19,2B1C,2B1F-2B29,2B30-2B39,2B40-2B49,2B50-2B55,1F30C,1F312-1F319,1F31F,1F321-1F322,1F32A,1F331-1F332,1F381,1F3A4,1F3AC,1F3B8,1F3BB,1F3C6,1F3F7,1F3F9,1F449,1F47B,1F481,1F4AB,1F4AD,1F4BC,1F4C5-1F4C6,1F4CC,1F4CE-1F4D0,1F4F8,1F4FC,1F508,1F51C,1F54A,1F5FF,1F6AA,1F6D0,1F6F0,1F915"
        cmap = self.font["cmap"]
        codes = cmap.getBestCmap().keys()
        character_sequence = get_unicode_character_sequence(codes)
        self.assertEqual(valid_sequence, character_sequence, "wrong character sequence")

if __name__ == '__main__':
    unittest.main(argv=['first-arg-is-ignored'], exit=False)

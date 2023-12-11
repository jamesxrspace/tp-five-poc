import { IconButton, Menu, MenuButton, MenuItem, MenuList } from '@chakra-ui/react';
import { useTranslation } from 'react-i18next';
import { RiTranslate2 } from 'react-icons/ri';
import { SUPPRTED_LANGUAGES } from './LanguageMenu.constant';

const LanguageMenu = () => {
  const { i18n } = useTranslation();

  return (
    <Menu placement="bottom-end">
      <MenuButton as={IconButton} variant="ghost" icon={<RiTranslate2 />}>
        Actions
      </MenuButton>
      <MenuList minW="none">
        {SUPPRTED_LANGUAGES.map((lang) => (
          <MenuItem
            key={lang.code}
            onClick={() => i18n.changeLanguage(lang.code)}
            isDisabled={lang.code === i18n.language}
            color={lang.code === i18n.language ? 'purple.400' : 'inherit'}
            _disabled={{ opacity: 1 }}
          >
            {lang.name}
          </MenuItem>
        ))}
      </MenuList>
    </Menu>
  );
};
export default LanguageMenu;

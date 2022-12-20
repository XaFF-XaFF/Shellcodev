#pragma once

#include <string>
#include <vector>
#include <algorithm>
#include <sstream>
#include <cctype>
#include <locale>

void ltrim(std::string &s);
void rtrim(std::string &s);
void trim(std::string &s);
bool is_number(const std::string& s);

std::vector<std::string> split(const std::string &str, const std::string &delim);
std::string join(const std::vector<std::string> &elements, const std::string &separator);

template<const unsigned num, const char separator>
static void separate(std::string & input)
{
	for (auto it = input.begin(); (num + 1) <= std::distance(it, input.end()); ++it)
	{
		std::advance(it, num);
		it = input.insert(it, separator);
	}
}

static unsigned char hex_char_to_byte(char Input)
{
	return
		((Input >= 'a') && (Input <= 'f'))
		? (Input - 87)
		: ((Input >= 'A') && (Input <= 'F'))
		? (Input - 55)
		: ((Input >= '0') && (Input <= '9'))
		? (Input - 48)
		: 0;//throw std::exception{};
}

/* Position the characters into the appropriate nibble */
static unsigned char transform_hex_to_byte(char High, char Low)
{
	return (hex_char_to_byte(High) << 4) | (hex_char_to_byte(Low));
}

template <typename InputIterator>
static std::string from_hex(InputIterator first, InputIterator last)
{
	std::ostringstream oss;

	while (first != last)
	{
		char highValue = *first++;
		if (highValue == ' ')
			continue;

		if (first == last)
			break;

		char lowValue = *first++;

		//char ch = (hex_to_byte::high(highValue) | hex_to_byte::low(lowValue));
		unsigned char ch = transform_hex_to_byte(highValue, lowValue);
		oss << ch;
	}

	return oss.str();
}
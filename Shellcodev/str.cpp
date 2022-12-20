#include "str.h"


void ltrim(std::string &s)
{
	s.erase(s.begin(), std::find_if(s.begin(), s.end(), [](int ch) {
		return !std::isspace(ch);
	}));
}

// trim from end (in place)
void rtrim(std::string &s)
{
	s.erase(std::find_if(s.rbegin(), s.rend(), [](int ch) {
		return !std::isspace(ch);
	}).base(), s.end());
}

void trim(std::string &s)
{
	ltrim(s);
	rtrim(s);
}

std::vector<std::string> split(const std::string &str, const std::string &delim)
{
	const auto delim_pos = str.find(delim);

	//std::string two = delim + delim;
	//std::replace(str.begin(), str.end(), two, delim);

	if (delim_pos == std::string::npos)
		return{ str };

	std::vector<std::string> ret{ str.substr(0, delim_pos) };
	auto tail = split(str.substr(delim_pos + delim.size(), std::string::npos), delim);

	ret.insert(ret.end(), tail.begin(), tail.end());

	return ret;
}

std::string join(const std::vector<std::string> &elements, const std::string &separator)
{
	if (!elements.empty())
	{
		std::stringstream ss;
		auto it = elements.cbegin();
		while (true)
		{
			ss << *it++;
			if (it != elements.cend())
				ss << separator;
			else
				return ss.str();
		}
	}
	return "";
}

bool is_number(const std::string& s)
{
	std::string::const_iterator it = s.begin();
	while (it != s.end() && std::isdigit(*it)) ++it;
	return !s.empty() && it == s.end();
}
/*****************************************************************************
*                                                                            *
*						|| COMPILER CONSTRUCTION ||                          *
*																			 *
*						*** TABLE DRIVEN PARSER ***						     *
*																			 *
*  NAME: Mudassar Ali                                                        *
*  Roll: Bcsf15m045                                                          *
*  Date: 02 - Jan - 2018                                                     *
*  Assignment # 04															 *
*																			 *
* ****************************************************************************/
#include <iostream>
#include <string>
#include <stack>
using namespace std;

size_t GetInputIndex(char);
size_t GetStackIndex(char);
void ShowError(char);
void ShowTitle();
void InvertPush(string);

stack <char> Stack;
string InputCharater = "vn+*-/()$";
string ActionCharater = "EATBFvn+*-/()$";
int ErrorCounter = 1;
string RawInput;
string parseTable[14][9] =
{ 
	{ "TA","TA","","","","","TA","Sync","Sync" },
	{ "","","+TA","","-TA","","","Null","Null" },
	{ "FB","FB","Sync","","Sync","","FB","Sync","Sync" },
	{ "","","Null","*FB","Null","/FB","","Null","Null" },
	{ "v","n","Sync","Sync","Sync","Sync","(E)","Sync","Sync" },
	{ "func(P,A)","","","","","","","","" },
	{ "","func(P,A)","","","","","","","" },
	{ "","","func(P,A)","","","","","","" },
	{ "","","","func(P,A)","","","","","" },
	{ "","","","","func(P,A)","","","","" },
	{ "","","","","","func(P,A)","","","" },
	{ "","","","","","","func(P,A)","","" },
	{ "","","","","","","","func(P,A)","" },
	{ "","","","","","","","","ACCEPT" }
};
int main()
{

	// program title
	ShowTitle();

	// setting up stack
	Stack.push('$');
	Stack.push('E');
	cout << "\n Enter Input Expression: ";
	cin >> RawInput;

	// appending $ at end of input
	RawInput.append("$");

	// start tracing parsing
	int i = 0;
	int InputIndex = 0;
	int StackIndex = 0;
	while (!Stack.empty())
	{

		// gettting indices

		InputIndex = GetInputIndex(RawInput[i]);
		StackIndex = GetStackIndex(Stack.top());

		//skippinh white spaces 
		if (RawInput[i] == ' ')
		{
			i++;
		}
		// checking for avaible indices
		else if (InputIndex == -1 || StackIndex == -1)
		{
			ShowError(RawInput[i++]);
		}
		// checking for epsilon, if epsilon then pop()
		else if (parseTable[StackIndex][InputIndex] == "Null")
		{
			Stack.pop();
		}
		// checking for empty cell
		else if (parseTable[StackIndex][InputIndex] == "")
		{
			ShowError(RawInput[i++]);
		}
		// checking for synchronized cell
		else if (parseTable[StackIndex][InputIndex] == "Sync")
		{
			ShowError(RawInput[i++]);
		}
		// checking for pop() and adv()
		else if (parseTable[StackIndex][InputIndex] == "func(P,A)")
		{
			Stack.pop();
			i++;
		}
		else if (parseTable[StackIndex][InputIndex] == "ACCEPT")
		{
			Stack.pop();
			cout << " Input " << RawInput.replace(RawInput.find('$'),1,"") << " ACCEPT" << endl;
		}
		// Replace(Inv()) to get Next-Action
		else
		{
			Stack.pop();
			string action = parseTable[StackIndex][InputIndex];
			InvertPush(action);
		}
	}
	
	system("pause");
	return 0;
}
size_t GetInputIndex(char ch)
{
	if (InputCharater.find(ch) >= 0 && InputCharater.find(ch) <=8)
	{
		return InputCharater.find(ch);
	}
	return -1;

}
size_t GetStackIndex(char ch)
{
	if (ActionCharater.find(ch) >= 0 && ActionCharater.find(ch) <=13)
	{
		return ActionCharater.find(ch);
	}
	return -1;
}
void InvertPush(string str)
{
	int len = str.length() - 1;
	while (len >= 0)
	{
		Stack.push(str[len]);
		len--;
	}
}
void ShowError(char ch)
{
	cout << " Error no. " << ErrorCounter++ << " . skip " << ch << endl;
}
void ShowTitle()
{
	cout << "*****************************************************************************"<<endl;
	cout << "*                                                                           *"<<endl;
	cout << "*                           TABLE DRIVEN PARSER                             *"<<endl;
	cout << "*                                                                           *"<<endl;
	cout << "*****************************************************************************"<<endl;
	cout << endl;
}

